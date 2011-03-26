﻿namespace SnittListan.Infrastructure
{
	using System;
	using System.Web.Security;
	using SnittListan.Services;

	/// <summary>
	/// Handles account membership.
	/// </summary>
	public class AccountMembershipService : IMembershipService
	{
		private readonly MembershipProvider provider;

		public AccountMembershipService()
			: this(null)
		{
		}

		public AccountMembershipService(MembershipProvider provider)
		{
			this.provider = provider ?? Membership.Provider;
		}

		public int MinPasswordLength
		{
			get
			{
				return provider.MinRequiredPasswordLength;
			}
		}

		public bool ValidateUser(string userName, string password)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(password))
				throw new ArgumentException("Value cannot be null or empty.", "password");

			return provider.ValidateUser(userName, password);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(password))
				throw new ArgumentException("Value cannot be null or empty.", "password");
			if (String.IsNullOrEmpty(email))
				throw new ArgumentException("Value cannot be null or empty.", "email");

			MembershipCreateStatus status;
			provider.CreateUser(userName, password, email, null, null, true, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(oldPassword))
				throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
			if (String.IsNullOrEmpty(newPassword))
				throw new ArgumentException("Value cannot be null or empty.", "newPassword");

			// The underlying ChangePassword() will throw an exception rather
			// than return false in certain failure scenarios.
			try
			{
				MembershipUser currentUser = provider.GetUser(userName, true /* userIsOnline */);
				return currentUser.ChangePassword(oldPassword, newPassword);
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (MembershipPasswordException)
			{
				return false;
			}
		}
	}
}