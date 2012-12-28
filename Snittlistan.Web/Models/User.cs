﻿namespace Snittlistan.Web.Models
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Raven.Imports.Newtonsoft.Json;

    using Snittlistan.Web.Events;

    /// <summary>
    /// Represents a registered user.
    /// </summary>
    public class User
    {
        private const string ConstantSalt = "CheFe2ra8en9SW";

        /// <summary>
        /// The salt needs to be initialized lazily.
        /// This allows it to be set when it is first needed (new user),
        /// and also when reconstituting (loading an existing user).
        /// </summary>
        private Guid passwordSalt;
        private string activationKey;

        /// <summary>
        /// Initializes a new instance of the User class.
        /// </summary>
        /// <param name="firstName">First name of the user.</param>
        /// <param name="lastName">Last name of the user.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">User password.</param>
        public User(string firstName, string lastName, string email, string password)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.HashedPassword = this.ComputeHashedPassword(password);
        }

        /// <summary>
        /// Initializes a new instance of the User class. This constructor is
        /// used when deserializing an entity stored in Raven.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="hashedPassword"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="requiresPasswordChange">Indicates whether this user requires password change.</param>
        [JsonConstructor]
// ReSharper disable UnusedMember.Local Used by Raven when loading.
        private User(
            string firstName,
            string lastName,
            string email,
            string hashedPassword,
            Guid passwordSalt,
            bool requiresPasswordChange)
// ReSharper restore UnusedMember.Local
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.HashedPassword = hashedPassword;
            this.passwordSalt = passwordSalt;
            this.RequiresPasswordChange = requiresPasswordChange;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is activated.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the activation key.
        /// </summary>
        public string ActivationKey
        {
            get { return this.activationKey ?? (this.activationKey = Guid.NewGuid().ToString()); }

            private set { this.activationKey = value; }
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets or sets the hashed password.
        /// </summary>
        private string HashedPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password change is required.
        /// </summary>
        private bool RequiresPasswordChange { get; set; }

        /// <summary>
        /// Gets the password salt, per user.
        /// </summary>
        private Guid PasswordSalt
        {
            get
            {
                if (this.passwordSalt == default(Guid))
                    this.passwordSalt = Guid.NewGuid();
                return this.passwordSalt;
            }
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="password">New password.</param>
        /// <param name="suppliedActivationKey">Supplied activation key.</param>
        public void SetPassword(string password, string suppliedActivationKey)
        {
            if (RequiresPasswordChange == false) throw new InvalidOperationException("Password change not allowed");
            if (ActivationKey != suppliedActivationKey) throw new InvalidOperationException("Unknown activation key");
            this.HashedPassword = this.ComputeHashedPassword(password);
            RequiresPasswordChange = false;
        }

        /// <summary>
        /// Validates a password against the user password.
        /// </summary>
        /// <param name="somePassword">Password to validate.</param>
        /// <returns>True if valid; false otherwise.</returns>
        public bool ValidatePassword(string somePassword)
        {
            return RequiresPasswordChange == false
                && this.HashedPassword == this.ComputeHashedPassword(somePassword);
        }

        /// <summary>
        /// Initializes a new user. Must be done for new users.
        /// </summary>
        public void Initialize()
        {
            this.ActivationKey = Guid.NewGuid().ToString();
            DomainEvent.Raise(new NewUserCreatedEvent { User = this });
        }

        /// <summary>
        /// Activates a user. This allows them to log on.
        /// </summary>
        public void Activate(bool invite)
        {
            this.IsActive = true;
            if (invite == false) return;
            this.ActivationKey = Guid.NewGuid().ToString();
            this.RequiresPasswordChange = true;
            DomainEvent.Raise(new UserInvitedEvent(this));
        }

        /// <summary>
        /// Deactivates a user. This prevents them from logging on.
        /// </summary>
        public void Deactivate()
        {
            this.IsActive = false;
        }

        /// <summary>
        /// Computes a hashed password.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        /// <returns>Hashed password.</returns>
        private string ComputeHashedPassword(string password)
        {
            string hashedPassword;
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(
                    this.PasswordSalt.ToByteArray().Concat(
                        Encoding.Unicode.GetBytes(password + this.PasswordSalt + ConstantSalt))
                        .ToArray());

                hashedPassword = Convert.ToBase64String(computedHash);
            }

            return hashedPassword;
        }
    }
}