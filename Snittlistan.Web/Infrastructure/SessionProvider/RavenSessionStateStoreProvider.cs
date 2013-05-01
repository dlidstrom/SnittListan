﻿namespace Snittlistan.Web.Infrastructure.SessionProvider
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.SessionState;

    using NLog;

    using Raven.Abstractions.Exceptions;
    using Raven.Client;
    using Raven.Json.Linq;

    /// <summary>
    /// An ASP.NET session-state store-provider implementation (http://msdn.microsoft.com/en-us/library/ms178588.aspx) using 
    /// RavenDb (http://ravendb.net) for persistence.
    /// </summary>
    public class RavenSessionStateStoreProvider : SessionStateStoreProviderBase
    {
        private const int RetriesOnConcurrentConfictsDefault = 3;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Func<IDocumentStore> storeLocator;
        private SessionStateSection sessionStateConfig;
        private int retriesOnConcurrentConflicts = RetriesOnConcurrentConfictsDefault;

        /// <summary>
        /// Initializes a new instance of the RavenSessionStateStoreProvider class.
        /// </summary>
        public RavenSessionStateStoreProvider()
        { }

        /// <summary>
        /// Initializes a new instance of the RavenSessionStateStoreProvider class, used for testing.
        /// </summary>
        /// <param name="documentStore">Document store.</param>
        public RavenSessionStateStoreProvider(IDocumentStore documentStore)
        {
            this.storeLocator = () => documentStore;
        }

        /// <summary>
        /// Gets or sets the name of the application. Session-data items will be stored against this name.
        /// If not set, defaults to System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.
        /// </summary>
        public string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            try
            {
                if (string.IsNullOrEmpty(name))
                    name = "RavenSessionStateStore";

                Logger.Debug(
                    "Beginning Initialize. Name={0}. Config={1}",
                    name,
                    config.AllKeys.Aggregate(string.Empty, (aggregate, next) => string.Format("{0}{1}:{2}", aggregate, next, config[next])));

                base.Initialize(name, config);

                int retries;
                if (int.TryParse(config["retriesOnConcurrentConflicts"], out retries))
                    this.retriesOnConcurrentConflicts = retries;

                this.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
                if (string.IsNullOrWhiteSpace(this.ApplicationName))
                    this.ApplicationName = HostingEnvironment.ApplicationVirtualPath;

                this.sessionStateConfig = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

                if (this.storeLocator == null)
                    this.storeLocator = () => MvcApplication.Container.Resolve<IDocumentStore>();

                Logger.Debug("Completed Initalize");
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error while initializing", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves session values and information from the session data store and locks the session-item data 
        /// at the data store for the duration of the request. 
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="locked">An output parameter indicating whether the item is currently exclusively locked.</param>
        /// <param name="lockAge">The age of the exclusive lock (if present).</param>
        /// <param name="lockId">The identifier of the exclusive lock (if present).</param>
        /// <param name="actions">Used with sessions whose Cookieless property is true, 
        /// when the regenerateExpiredSessionId attribute is set to true. 
        /// An actionFlags value set to InitializeItem (1) indicates that the entry in the session data store is a 
        /// new session that requires initialization.</param>
        /// <returns>The session data.</returns>
        public override SessionStateStoreData GetItemExclusive(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actions)
        {
            try
            {
                Logger.Debug("Beginning GetItemExclusive. SessionId={0}", id);

                var item = this.GetSessionStoreItem(
                    true, context, id, this.retriesOnConcurrentConflicts, out locked, out lockAge, out lockId, out actions);

                Logger.Debug("Completed GetItemExclusive. SessionId={0}, locked={1}, lockAge={2}, lockId={3}, actions={4}", id, locked, lockAge, lockId, actions);

                return item;
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Error during GetItemExclusive. SessionId={0}", id), ex);
                throw;
            }
        }

        /// <summary>
        /// This method performs the same work as the GetItemExclusive method, except that it does not attempt to lock the session item in the data store.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="locked">An output parameter indicating whether the item is currently exclusively locked.</param>
        /// <param name="lockAge">The age of the exclusive lock (if present).</param>
        /// <param name="lockId">The identifier of the exclusive lock (if present).</param>
        /// <param name="actions">Used with sessions whose Cookieless property is true,
        /// when the regenerateExpiredSessionId attribute is set to true.
        /// An actionFlags value set to InitializeItem (1) indicates that the entry in the session data store is a
        /// new session that requires initialization.</param>
        /// <returns>The session data.</returns>
        public override SessionStateStoreData GetItem(
            HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            try
            {
                Logger.Debug("Beginning GetItem. SessionId={0}", id);

                var item = this.GetSessionStoreItem(false, context, id, 0, out locked, out lockAge, out lockId, out actions);

                Logger.Debug("Completed GetItem. SessionId={0}, locked={1}, lockAge={2}, lockId={3}, actions={4}", id, locked, lockAge, lockId, actions);

                return item;
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Error during GetItem. SessionId={0}", id), ex);
                throw;
            }
        }

        /// <summary>
        /// If the newItem parameter is true, the SetAndReleaseItemExclusive method inserts a new item into the data store with the supplied values. 
        /// Otherwise, the existing item in the data store is updated with the supplied values, and any lock on the data is released.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="item">The current session values to be stored.</param>
        /// <param name="lockId">The lock identifier for the current request.</param>
        /// <param name="newItem">If true, a new item is inserted into the store. Otherwise, the existing item in
        /// the data store is updated with the supplied values, and any lock on the data is released.</param>
        public override void SetAndReleaseItemExclusive(
            HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            try
            {
                Logger.Debug(
                    " Beginning SetAndReleaseItemExclusive. SessionId={0}, LockId={1}, newItem={2}", id, lockId, newItem);

                var serializedItems = this.Serialize((SessionStateItemCollection)item.Items);

                var store = this.storeLocator();
                using (var documentSession = store.OpenSession())
                {
                    // if we get a concurrency conflict, then we want to know about it
                    documentSession.Advanced.UseOptimisticConcurrency = true;

                    SessionState sessionState;
                    if (newItem)
                    {
                        sessionState = documentSession.Query<SessionState>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                            .SingleOrDefault(x => x.SessionId == id && x.ApplicationName == ApplicationName && x.Expires < DateTime.UtcNow);

                        if (sessionState != null)
                            throw new InvalidOperationException(string.Format("Item aleady exist with SessionId={0} and ApplicationName={1}", id, lockId));

                        sessionState = new SessionState(id, this.ApplicationName);
                        documentSession.Store(sessionState);
                    }
                    else
                    {
                        sessionState = documentSession.Query<SessionState>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                            .Single(x => x.SessionId == id && x.ApplicationName == this.ApplicationName && x.LockId == (int)lockId);
                    }

                    var expiryDate = DateTime.UtcNow.AddMinutes(this.sessionStateConfig.Timeout.TotalMinutes);
                    var ravenJObject = documentSession.Advanced.GetMetadataFor(sessionState);
                    ravenJObject["Raven-Expiration-Date"] = new RavenJValue(expiryDate);
                    sessionState.Expires = expiryDate;
                    sessionState.SessionItems = serializedItems;
                    sessionState.Locked = false;

                    documentSession.SaveChanges();
                }

                Logger.Debug("Completed SetAndReleaseItemExclusive. SessionId={0}, LockId={1}, newItem={2}", id, lockId, newItem);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Error during SetAndReleaseItemExclusive. SessionId={0}, LockId={1}, newItem={2}", id, lockId, newItem), ex);
                throw;
            }
        }

        /// <summary>
        /// Releases the lock on an item in the session data store.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="lockId">The lock identifier for the current request.</param>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            try
            {
                Logger.Debug("Beginning ReleaseItemExclusive. SessionId={0}, LockId={1}", id, lockId);

                var store = this.storeLocator();
                using (var documentSession = store.OpenSession())
                {
                    // if we get a concurrency conflict, then we want to know about it
                    documentSession.Advanced.UseOptimisticConcurrency = true;

                    var sessionState = documentSession.Query<SessionState>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                        .Single(x => x.SessionId == id && x.ApplicationName == this.ApplicationName && x.LockId == (int)lockId);

                    sessionState.Locked = false;

                    var expiry = DateTime.UtcNow.AddMinutes(this.sessionStateConfig.Timeout.TotalMinutes);
                    sessionState.Expires = expiry;
                    documentSession.Advanced.GetMetadataFor(sessionState)["Raven-Expiration-Date"] =
                        new RavenJValue(expiry);

                    documentSession.SaveChanges();
                }

                Logger.Debug("Completed ReleaseItemExclusive. SessionId={0}, LockId={1}", id, lockId);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Error during ReleaseItemExclusive. SessionId={0}, LockId={1}", id, lockId), ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the session information from the data store where the data store item matches the supplied SessionID value, 
        /// the current application, and the supplied lock identifier.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="lockId">The exclusive-lock identifier.</param>
        /// <param name="item">Unused parameter.</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            try
            {
                Logger.Debug("Beginning RemoveItem. id={0}, lockId={1}", id, lockId);

                var store = this.storeLocator();
                using (var documentSession = store.OpenSession())
                {
                    // if we get a concurrency conflict, then we want to know about it
                    documentSession.Advanced.UseOptimisticConcurrency = true;

                    var sessionState =
                        documentSession.Query<SessionState>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                        .SingleOrDefault(
                            x => x.SessionId == id && x.ApplicationName == this.ApplicationName && x.LockId == (int)lockId);

                    if (sessionState != null)
                    {
                        documentSession.Delete(sessionState);
                        documentSession.SaveChanges();
                    }
                }

                Logger.Debug("Completed RemoveItem. id={0}, lockId={1}", id, lockId);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Error during RemoveItem. SessionId={0}; LockId={1}", id, lockId), ex);
                throw;
            }
        }

        /// <summary>
        /// Resets the expiry timeout for a session item.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            try
            {
                Logger.Debug("Beginning ResetItemTimeout. id={0}", id);

                var store = this.storeLocator();
                using (var documentSession = store.OpenSession())
                {
                    // we do not want to overwrite any changes
                    documentSession.Advanced.UseOptimisticConcurrency = true;

                    var sessionState = documentSession.Query<SessionState>().SingleOrDefault(
                        x => x.SessionId == id && x.ApplicationName == this.ApplicationName);

                    if (sessionState != null)
                    {
                        var expiry = DateTime.UtcNow.AddMinutes(this.sessionStateConfig.Timeout.TotalMinutes);
                        sessionState.Expires = expiry;
                        documentSession.Advanced.GetMetadataFor(sessionState)["Raven-Expiration-Date"] =
                            new RavenJValue(expiry);
                        documentSession.SaveChanges();
                    }
                }

                Logger.Debug("Completed ResetItemTimeout. id={0}", id);
            }
            catch (ConcurrencyException cEx)
            {
                // we log and ignore. Should never happen, but not fatal if it does.
                Logger.ErrorException("ConcurrencyException during ResetTimeout. SessionId=" + id, cEx);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error during ResetItemTimeout. SessionId=" + id, ex);
                throw;
            }
        }

        /// <summary>
        /// Adds an uninitialized item to the session data store.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="timeout">The expiry timeout in minutes.</param>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            try
            {
                Logger.Debug("Beginning CreateUninitializedItem. id={0}, timeout={1}", id, timeout);

                var store = this.storeLocator();
                using (var documentSession = store.OpenSession())
                {
                    var expiry = DateTime.UtcNow.AddMinutes(timeout);

                    var sessionState = new SessionState(id, this.ApplicationName)
                                           {
                                               Expires = expiry
                                           };

                    documentSession.Store(sessionState);
                    documentSession.Advanced.GetMetadataFor(sessionState)["Raven-Expiration-Date"] =
                        new RavenJValue(expiry);
                    documentSession.SaveChanges();
                }

                Logger.Debug("Completed CreateUninitializedItem. id={0}, timeout={1}", id, timeout);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error during CreateUninitializedItem", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns a new SessionStateStoreData object with an empty ISessionStateItemCollection object, 
        /// an HttpStaticObjectsCollection collection, and the specified Timeout value.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        /// <param name="timeout">The expiry timeout in minutes.</param>
        /// <returns>A newly created SessionStateStoreData object.</returns>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(
                new SessionStateItemCollection(),
                SessionStateUtility.GetSessionStaticObjects(context),
                timeout);
        }

        /// <summary>
        /// Takes as input a delegate that references the Session_OnEnd event defined in the Global.asax file. 
        /// If the session-state store provider supports the Session_OnEnd event, a local reference to the 
        /// SessionStateItemExpireCallback parameter is set and the method returns true; otherwise, the method returns false.
        /// </summary>
        /// <param name="expireCallback">A callback.</param>
        /// <returns>Always returns false.</returns>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        /// <summary>
        /// Performs any initialization required by your session-state store provider.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        public override void InitializeRequest(HttpContext context)
        {
        }

        /// <summary>
        /// Performs any cleanup required by your session-state store provider.
        /// </summary>
        /// <param name="context">The HttpContext instance for the current request.</param>
        public override void EndRequest(HttpContext context)
        {
        }

        public override void Dispose()
        {
        ////    try
        ////    {
        ////        if (documentStore != null)
        ////            documentStore.Dispose();
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        // swallow the exception...nothing good can come from throwing it here!
        ////        Logger.ErrorException("An exception was thrown while disposing the DocumentStore: ", ex);
        ////    }
        }

        private static SessionStateStoreData Deserialize(
            HttpContext context, string serializedItems, int timeout)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(serializedItems)))
            {
                var sessionItems = new SessionStateItemCollection();

                if (stream.Length > 0)
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        sessionItems = SessionStateItemCollection.Deserialize(reader);
                    }
                }

                return new SessionStateStoreData(
                    sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout);
            }
        }

        /// <summary>
        /// GetSessionStoreItem is called by both the GetItem and 
        /// GetItemExclusive methods. GetSessionStoreItem retrieves the 
        /// session data from the data source. If the lockRecord parameter
        /// is true (in the case of GetItemExclusive), then GetSessionStoreItem
        /// locks the record and sets a new LockId and LockDate.
        /// </summary>
        /// <param name="lockRecord"></param>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="retriesRemaining"></param>
        /// <param name="locked"></param>
        /// <param name="lockAge"></param>
        /// <param name="lockId"></param>
        /// <param name="actionFlags"></param>
        /// <returns></returns>
        private SessionStateStoreData GetSessionStoreItem(
            bool lockRecord,
            HttpContext context,
            string id,
            int retriesRemaining,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            // Initial values for return value and out parameters.
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;

            var store = this.storeLocator();
            using (var documentSession = store.OpenSession())
            {
                // don't tolerate stale data
                documentSession.Advanced.AllowNonAuthoritativeInformation = false;

                // if we get a concurrency conflict, then we want to know about it
                documentSession.Advanced.UseOptimisticConcurrency = true;

                Logger.Debug("Retrieving item from RavenDB. SessionId: {0}; ApplicationName={1}", id, this.ApplicationName);

                var sessionState =
                    documentSession.Query<SessionState>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .SingleOrDefault(x => x.SessionId == id && x.ApplicationName == this.ApplicationName);

                if (sessionState == null)
                {
                    Logger.Debug("Item not found in RavenDB with SessionId: {0}; ApplicationName={1}", id, this.ApplicationName);
                    return null;
                }

                // if the record is locked, we can't have it.
                if (sessionState.Locked)
                {
                    Logger.Debug("Item retrieved is locked. SessionId: {0}; ApplicationName={1}", id, this.ApplicationName);

                    locked = true;
                    lockAge = DateTime.UtcNow.Subtract(sessionState.LockDate);
                    lockId = sessionState.LockId;
                    return null;
                }

                // generally we shouldn't get expired items, as the expiration bundle should clean them up,
                // but just in case the bundle isn't installed, or we made the window, we'll delete expired items here.
                if (sessionState.Expires < DateTime.UtcNow)
                {
                    Logger.Debug("Item retrieved has expired. SessionId: {0}; ApplicationName={1}; Expiry (UTC): {3}", id, this.ApplicationName, sessionState.Expires);

                    try
                    {
                        documentSession.Delete(sessionState);
                        documentSession.SaveChanges();
                    }
                    catch (ConcurrencyException)
                    {
                        // we swallow, as we don't care. Presumably the other modifier deleted it as well.
                    }

                    return null;
                }

                if (lockRecord)
                {
                    sessionState.Locked = true;
                    sessionState.LockId += 1;
                    sessionState.LockDate = DateTime.UtcNow;

                    try
                    {
                        documentSession.SaveChanges();
                    }
                    catch (ConcurrencyException)
                    {
                        if (retriesRemaining > 0)
                        {
                            return this.GetSessionStoreItem(
                                true,
                                context,
                                id,
                                retriesRemaining - 1,
                                out locked,
                                out lockAge,
                                out lockId,
                                out actionFlags);
                        }

                        throw;
                    }
                }

                lockId = sessionState.LockId;
                if (sessionState.Flags == SessionStateActions.InitializeItem)
                {
                    return new SessionStateStoreData(
                        new SessionStateItemCollection(),
                        SessionStateUtility.GetSessionStaticObjects(context),
                        (int)this.sessionStateConfig.Timeout.TotalMinutes);
                }

                return Deserialize(context, sessionState.SessionItems, (int)this.sessionStateConfig.Timeout.TotalMinutes);
            }
        }

        private string Serialize(SessionStateItemCollection items)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    if (items != null)
                        items.Serialize(writer);

                    writer.Flush();
                    writer.Close();

                    return Convert.ToBase64String(stream.ToArray());
                }
            }
        }
    }
}