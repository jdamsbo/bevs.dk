using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

using MySql.Data.MySqlClient;

/*

-- Please, send me an email (andriniaina@gmail.com) if you have done some improvements or bug corrections to this file
      or leave your modifications on the comments page on sprinj.com
	  
*/


namespace Andri.Web
{
	public sealed class MySqlMembershipProvider : MembershipProvider
	{

		//
		// Global connection string, generated password length, generic exception message, event log info.
		//

		private const int newPasswordLength = 5;
		private const string tableName = "Users";
		private string connectionString;

		private byte[] encryptionKey;// = new byte[] { 255, 30, 40, 20, 13, 59 };


		//
		// System.Configuration.Provider.ProviderBase.Initialize Method
		//
		public override void Initialize(string name, NameValueCollection config)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.Initialize");
			//
			// Initialize values from web.config.
			//

			if (config == null)
				throw new ArgumentNullException("config");

			if (name == null || name.Length == 0)
				name = "MySqlMembershipProvider";

			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "MySql Membership provider");
			}

			// Initialize the abstract base class.
			base.Initialize(name, config);

			pApplicationName = GetConfigValue(
				config["applicationName"],
				System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
				);
			pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
			pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
			pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
			pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
			pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
			pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
			try
			{
				encryptionKey = HexToByte(GetConfigValue(config["encryptionKey"], "AB56FE8EA700B42A"));
			}
			catch(Exception e) 
			{}

			string temp_format = config["passwordFormat"];
			if (temp_format == null)
			{
				temp_format = "Hashed";
			}

			switch (temp_format)
			{
				case "Hashed":
					pPasswordFormat = MembershipPasswordFormat.Hashed;
					break;
				case "Encrypted":
					pPasswordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Clear":
					pPasswordFormat = MembershipPasswordFormat.Clear;
					break;
				default:
					throw new ProviderException("Password format not supported.");
			}

			//
			// Initialize MySqlConnection.
			//
			ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
			if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim().Length == 0)
			{
				throw new ProviderException("Connection string cannot be blank.");
			}

			connectionString = ConnectionStringSettings.ConnectionString;
		}

		//
		// A helper function to retrieve config values from the configuration file.
		//
		private string GetConfigValue(string configValue, string defaultValue)
		{
			return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
		}


		//
		// System.Web.Security.MembershipProvider properties.
		//
		private string pApplicationName;
		public override string ApplicationName
		{
			get { return pApplicationName; }
			set { pApplicationName = value; }
		}

		private bool pEnablePasswordReset;
		public override bool EnablePasswordReset
		{
			get { return pEnablePasswordReset; }
		}


		private bool pEnablePasswordRetrieval;
		public override bool EnablePasswordRetrieval
		{
			get { return pEnablePasswordRetrieval; }
		}

		private bool pRequiresQuestionAndAnswer;
		public override bool RequiresQuestionAndAnswer
		{
			get { return pRequiresQuestionAndAnswer; }
		}


		private bool pRequiresUniqueEmail;
		public override bool RequiresUniqueEmail
		{
			get { return pRequiresUniqueEmail; }
		}

		private int pMaxInvalidPasswordAttempts;
		public override int MaxInvalidPasswordAttempts
		{
			get { return pMaxInvalidPasswordAttempts; }
		}

		private int pPasswordAttemptWindow;
		public override int PasswordAttemptWindow
		{
			get { return pPasswordAttemptWindow; }
		}

		private MembershipPasswordFormat pPasswordFormat;
		public override MembershipPasswordFormat PasswordFormat
		{
			get { return pPasswordFormat; }
		}

		private int pMinRequiredNonAlphanumericCharacters;
		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return pMinRequiredNonAlphanumericCharacters; }
		}

		private int pMinRequiredPasswordLength;
		public override int MinRequiredPasswordLength
		{
			get { return pMinRequiredPasswordLength; }
		}

		private string pPasswordStrengthRegularExpression;
		public override string PasswordStrengthRegularExpression
		{
			get { return pPasswordStrengthRegularExpression; }
		}

		//
		// System.Web.Security.MembershipProvider methods.
		//

		public override bool ChangePassword(string username, string oldPwd, string newPwd)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.ChangePassword");

			if (!ValidateUser(username, oldPwd))
				return false;

			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);
			OnValidatingPassword(args);

			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Change password canceled due to new password validation failure.");


			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("UPDATE `" + tableName + "`" +
						" SET Password = ?Password, LastPasswordChangedDate = ?LastPasswordChangedDate " +
						" WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(newPwd);
				cmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = DateTime.Now;
				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();
				conn.Close();

				return (rowsAffected > 0);
			}
		}



		//
		// MembershipProvider.ChangePasswordQuestionAndAnswer
		//

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPwdQuestion, string newPwdAnswer)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.ChangePasswordQuestionAndAnswer");

			if (!ValidateUser(username, password))
				return false;

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("UPDATE `" + tableName + "`" +
						" SET PasswordQuestion = ?Question, PasswordAnswer = ?Answer" +
						" WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Question", MySqlDbType.VarChar, 255).Value = newPwdQuestion;
				cmd.Parameters.Add("?Answer", MySqlDbType.VarChar, 255).Value = EncodePassword(newPwdAnswer);
				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;


				conn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();
				conn.Close();

				return rowsAffected > 0;
			}
		}

		//
		// MembershipProvider.CreateUser
		//
		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.CreateUser");

			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
			OnValidatingPassword(args);

			if (args.Cancel)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			string name = GetUserNameByEmail(email);
			if (RequiresUniqueEmail && null!=name)
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			if (null == GetUser(username, false))
			{
				DateTime createDate = DateTime.Now;

				if (providerUserKey == null)
				{
					providerUserKey = Guid.NewGuid();
				}
				else
				{
					if (!(providerUserKey is Guid))
					{
						status = MembershipCreateStatus.InvalidProviderUserKey;
						return null;
					}
				}

				using (MySqlConnection conn = new MySqlConnection(connectionString))
				{
					MySqlCommand cmd = new MySqlCommand("INSERT INTO `" + tableName + "`" +
						  " (UserId, Username, Password, Email, PasswordQuestion, " +
						  " PasswordAnswer, IsApproved," +
						  " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," +
						  " ApplicationName, IsLockedOut, LastLockedOutDate," +
						  " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " +
						  " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" +
						  " Values(?UserId, ?Username, ?Password, ?Email, ?PasswordQuestion, " +
						  " ?PasswordAnswer, ?IsApproved, ?Comment, ?CreationDate, ?LastPasswordChangedDate, " +
						  " ?LastActivityDate, ?ApplicationName, ?IsLockedOut, ?LastLockedOutDate, " +
						  " ?FailedPasswordAttemptCount, ?FailedPasswordAttemptWindowStart, " +
						  " ?FailedPasswordAnswerAttemptCount, ?FailedPasswordAnswerAttemptWindowStart)", conn);

					cmd.Parameters.Add("?UserId", MySqlDbType.VarChar).Value = providerUserKey.ToString();
					cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
					cmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(password);
					cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = email;
					cmd.Parameters.Add("?PasswordQuestion", MySqlDbType.VarChar, 255).Value = passwordQuestion;
					cmd.Parameters.Add("?PasswordAnswer", MySqlDbType.VarChar, 255).Value = passwordAnswer == null ? null : EncodePassword(passwordAnswer);
					cmd.Parameters.Add("?IsApproved", MySqlDbType.Bit).Value = isApproved;
					cmd.Parameters.Add("?Comment", MySqlDbType.VarChar, 255).Value = "";
					cmd.Parameters.Add("?CreationDate", MySqlDbType.Datetime).Value = createDate;
					cmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = createDate;
					cmd.Parameters.Add("?LastActivityDate", MySqlDbType.Datetime).Value = createDate;
					cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
					cmd.Parameters.Add("?IsLockedOut", MySqlDbType.Bit).Value = 0;	//false
					cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = createDate;
					cmd.Parameters.Add("?FailedPasswordAttemptCount", MySqlDbType.Int32).Value = 0;
					cmd.Parameters.Add("?FailedPasswordAttemptWindowStart", MySqlDbType.Datetime).Value = createDate;
					cmd.Parameters.Add("?FailedPasswordAnswerAttemptCount", MySqlDbType.Int32).Value = 0;
					cmd.Parameters.Add("?FailedPasswordAnswerAttemptWindowStart", MySqlDbType.Datetime).Value = createDate;

					conn.Open();

					int recAdded = cmd.ExecuteNonQuery();

					status = (recAdded > 0) ?
						MembershipCreateStatus.Success :
						MembershipCreateStatus.UserRejected;
					conn.Close();
				}

				return GetUser(username, false);
			}
			else
			{
				status = MembershipCreateStatus.DuplicateUserName;
			}

			return null;
		}



		//
		// MembershipProvider.DeleteUser
		//
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.DeleteUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("DELETE FROM `" + tableName + "`" +
						" WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();
				if (deleteAllRelatedData)
				{
					// nothing to do ? Process commands to delete all data for the user in the database.
				}
				conn.Close();

				return (rowsAffected > 0);
			}
		}



		//
		// MembershipProvider.GetAllUsers
		//

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetAllUsers");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MembershipUserCollection users = new MembershipUserCollection();


				conn.Open();

				int startIndex = pageSize * pageIndex;
				MySqlCommand cmd = new MySqlCommand("SELECT SQL_CALC_FOUND_ROWS UserId, Username, Email, PasswordQuestion," +
						 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
						 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
						 " FROM `" + tableName + "` " +
						 " WHERE ApplicationName = ?ApplicationName " +
						 " ORDER BY Username Asc" +
						 " LIMIT " + startIndex + "," + pageSize, conn);
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;


				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						users.Add(GetUserFromReader(reader));
					}
					reader.Close();
				}

				cmd.CommandText = "SELECT FOUND_ROWS()";
				totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
				conn.Close();

				return users;
			}
		}

		//
		// MembershipProvider.GetNumberOfUsersOnline
		//
		public override int GetNumberOfUsersOnline()
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetNumberOfUsersOnline");

			TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
			DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT Count(*) FROM `" + tableName + "`" +
						" WHERE LastActivityDate > ?CompareDate AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?CompareDate", MySqlDbType.Datetime).Value = compareTime;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;


				conn.Open();

				int numOnline = Convert.ToInt32(cmd.ExecuteScalar());
				conn.Close();

				return numOnline;
			}
		}


		//
		// MembershipProvider.GetPassword
		//
		public override string GetPassword(string username, string answer)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetPassword");

			if (!EnablePasswordRetrieval)
			{
				throw new ProviderException("Password Retrieval Not Enabled.");
			}

			if (PasswordFormat == MembershipPasswordFormat.Hashed)
			{
				throw new ProviderException("Cannot retrieve Hashed passwords.");
			}

			string password = "";
			string passwordAnswer = "";
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM `" + tableName + "`" +
					  " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();

				using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						reader.Read();

						if (reader.GetBoolean(2))
							throw new MembershipPasswordException("The supplied user is locked out.");

						password = reader.GetString(0);
						passwordAnswer = reader.GetString(1);
					}
					else
					{
						throw new MembershipPasswordException("The supplied user name is not found.");
					}
					reader.Close();
				}
				conn.Close();
			}

			if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
			{
				UpdateFailureCount(username, "passwordAnswer");

				throw new MembershipPasswordException("Incorrect password answer.");
			}


			if (PasswordFormat == MembershipPasswordFormat.Encrypted)
			{
				password = UnEncodePassword(password);
			}

			return password;
		}

		//
		// MembershipProvider.GetUser(string, bool)
		//
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MembershipUser u = null;
				MySqlCommand cmd = new MySqlCommand("SELECT UserId, Username, Email, PasswordQuestion," +
					 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
					 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
					 " FROM `" + tableName + "` WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						u = GetUserFromReader(reader);
						reader.Close();
					}
				}
				if (userIsOnline)
				{
					MySqlCommand updateCmd = new MySqlCommand("UPDATE `" + tableName + "` " +
							  " SET LastActivityDate = ?LastActivityDate " +
							  " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

					updateCmd.Parameters.Add("?LastActivityDate", MySqlDbType.VarChar).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
					updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
					updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

					updateCmd.ExecuteNonQuery();
				}
				conn.Close();

				return u;
			}

		}


		//
		// MembershipProvider.GetUser(object, bool)
		//
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT UserId, Username, Email, PasswordQuestion," +
					  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
					  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
					  " FROM `" + tableName + "` WHERE UserId = ?UserId", conn);

				cmd.Parameters.Add("?UserId", MySqlDbType.VarChar).Value = providerUserKey;

				MembershipUser u = null;

				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						u = GetUserFromReader(reader);
						reader.Close();
					}
					reader.Close();
				}

				if (userIsOnline)
				{
					MySqlCommand updateCmd = new MySqlCommand("UPDATE `" + tableName + "` " +
							  " SET LastActivityDate = ?LastActivityDate " +
							  " WHERE UserId = ?UserId", conn);

					updateCmd.Parameters.Add("?LastActivityDate", MySqlDbType.VarChar).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
					updateCmd.Parameters.Add("?UserId", MySqlDbType.VarChar).Value = providerUserKey;
					updateCmd.ExecuteNonQuery();
				}
				conn.Close();

				return u;
			}

		}


		//
		// GetUserFromReader
		//    A helper function that takes the current row from the MySqlDataReader
		//    and hydrates a MembershiUser from the values. Called by the 
		//    MembershipUser.GetUser implementation.
		//
		private MembershipUser GetUserFromReader(MySqlDataReader reader)
		{
			object providerUserKey = new Guid(reader.GetValue(0).ToString());
			string username = reader.IsDBNull(1) ? "" : reader.GetString(1);
			string email = reader.IsDBNull(2) ? "" : reader.GetString(2);
			string passwordQuestion = reader.IsDBNull(3) ? "" : reader.GetString(3);
			string comment = reader.IsDBNull(4) ? "" : reader.GetString(4);
			bool isApproved = reader.IsDBNull(5) ? false : reader.GetBoolean(5);
			bool isLockedOut = reader.IsDBNull(6) ? false : reader.GetBoolean(6);
			DateTime creationDate = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);
			DateTime lastLoginDate = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8);
			DateTime lastActivityDate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9);
			DateTime lastPasswordChangedDate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10);
			DateTime lastLockedOutDate = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);

			return new MembershipUser(
				this.Name,
				username,
				providerUserKey,
				email,
				passwordQuestion,
				comment,
				isApproved,
				isLockedOut,
				creationDate,
				lastLoginDate,
				lastActivityDate,
				lastPasswordChangedDate,
				lastLockedOutDate
				);
		}


		//
		// MembershipProvider.UnlockUser
		//

		public override bool UnlockUser(string username)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.UnlockUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("UPDATE `" + tableName + "` " +
												  " SET IsLockedOut = 0, LastLockedOutDate = ?LastLockedOutDate " +
												  " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = DateTime.Now;
				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();
				conn.Close();

				return (rowsAffected > 0);
			}
		}


		//
		// MembershipProvider.GetUserNameByEmail
		//
		public override string GetUserNameByEmail(string email)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.GetUserNameByEmail");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT Username" +
					  " FROM `" + tableName + "` WHERE Email = ?Email AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = email;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				string username = cmd.ExecuteScalar() as string;
				conn.Close();

				return username;
			}

		}

		//
		// MembershipProvider.ResetPassword
		//
		public override string ResetPassword(string username, string answer)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.ResetPassword");

			if (!EnablePasswordReset)
			{
				throw new NotSupportedException("Password reset is not enabled.");
			}

			if (answer == null && RequiresQuestionAndAnswer)
			{
				UpdateFailureCount(username, "passwordAnswer");

				throw new ProviderException("Password answer required for password reset.");
			}

			string newPassword = System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
			OnValidatingPassword(args);

			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT PasswordAnswer, IsLockedOut FROM `" + tableName + "`" +
					  " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				string passwordAnswer = "";

				conn.Open();

				using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						reader.Read();

						if (reader.GetBoolean(1))
							throw new MembershipPasswordException("The supplied user is locked out.");

						passwordAnswer = reader.GetString(0);
					}
					else
					{
						throw new MembershipPasswordException("The supplied user name is not found.");
					}
					reader.Close();
				}

				if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
				{
					UpdateFailureCount(username, "passwordAnswer");

					throw new MembershipPasswordException("Incorrect password answer.");
				}

				MySqlCommand updateCmd = new MySqlCommand("UPDATE `" + tableName + "`" +
					" SET Password = ?Password, LastPasswordChangedDate = ?LastPasswordChangedDate" +
					" WHERE Username = ?Username AND ApplicationName = ?ApplicationName AND IsLockedOut = 0", conn);

				updateCmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(newPassword);
				updateCmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = DateTime.Now;
				updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				int rowsAffected = updateCmd.ExecuteNonQuery();
				conn.Close();

				if (rowsAffected > 0)
				{
					return newPassword;
				}
				else
				{
					throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
				}
			}
		}


		//
		// MembershipProvider.UpdateUser
		//
		public override void UpdateUser(MembershipUser user)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.UpdateUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("UPDATE `" + tableName + "`" +
						" SET Email = ?Email, Comment = ?Comment," +
						" IsApproved = ?IsApproved" +
						" WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = user.Email;
				cmd.Parameters.Add("?Comment", MySqlDbType.VarChar, 255).Value = user.Comment;
				cmd.Parameters.Add("?IsApproved", MySqlDbType.Bit).Value = user.IsApproved;
				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = user.UserName;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}


		//
		// MembershipProvider.ValidateUser
		//

		public override bool ValidateUser(string username, string password)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.ValidateUser");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				bool isValid = false;

				MySqlCommand cmd = new MySqlCommand("SELECT Password, IsApproved FROM `" + tableName + "`" +
						" WHERE Username = ?Username AND ApplicationName = ?ApplicationName AND IsLockedOut = 0", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				bool isApproved = false;
				string pwd = "";


				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						reader.Read();
						pwd = reader.GetString(0);
						isApproved = reader.GetBoolean(1);
					}
					else
					{
						return false;
					}
					reader.Close();
				}

				if (CheckPassword(password, pwd))
				{
					if (isApproved)
					{
						isValid = true;

						MySqlCommand updateCmd = new MySqlCommand("UPDATE `" + tableName + "` SET LastLoginDate = ?LastLoginDate, LastActivityDate = ?LastActivityDate" +
																" WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

						updateCmd.Parameters.Add("?LastLoginDate", MySqlDbType.Datetime).Value = DateTime.Now;
						updateCmd.Parameters.Add("?LastActivityDate", MySqlDbType.Datetime).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
						updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
						updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

						updateCmd.ExecuteNonQuery();
					}
				}
				else
				{
					conn.Close();

					UpdateFailureCount(username, "password");
				}
				conn.Close();
				return isValid;
			}

		}


		//
		// UpdateFailureCount
		//   A helper method that performs the checks and updates associated with
		// password failure tracking.
		//
		private void UpdateFailureCount(string username, string failureType)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.UpdateFailureCount");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MySqlCommand cmd = new MySqlCommand("SELECT FailedPasswordAttemptCount, " +
												  "  FailedPasswordAttemptWindowStart, " +
												  "  FailedPasswordAnswerAttemptCount, " +
												  "  FailedPasswordAnswerAttemptWindowStart " +
												  "  FROM `" + tableName + "` " +
												  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);

				cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				DateTime windowStart = new DateTime();
				int failureCount = 0;

				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						reader.Read();
						switch (failureType)
						{
							case "password":
								failureCount = reader.GetInt32(0);
								windowStart = reader.GetDateTime(1);
								break;
							case "passwordAnswer":
								failureCount = reader.GetInt32(2);
								windowStart = reader.GetDateTime(3);
								break;
						}
					}
					reader.Close();
				}

				DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

				if (failureCount == 0 || DateTime.Now > windowEnd)
				{
					// First password failure or outside of PasswordAttemptWindow. 
					// Start a new password failure count from 1 and a new window starting now.

					switch (failureType)
					{
						case "password":
							cmd.CommandText = "UPDATE `" + tableName + "` " +
											  "  SET FailedPasswordAttemptCount = ?Count, " +
											  "      FailedPasswordAttemptWindowStart = ?WindowStart " +
											  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
							break;
						case "passwordAnswer":
							cmd.CommandText = "UPDATE `" + tableName + "` " +
											  "  SET FailedPasswordAnswerAttemptCount = ?Count, " +
											  "      FailedPasswordAnswerAttemptWindowStart = ?WindowStart " +
											  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
							break;
					}


					cmd.Parameters.Clear();

					cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = 1;
					cmd.Parameters.Add("?WindowStart", MySqlDbType.Datetime).Value = DateTime.Now;
					cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
					cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

					if (cmd.ExecuteNonQuery() < 0)
						throw new ProviderException("Unable to update failure count and window start.");
				}
				else if (failureCount++ >= MaxInvalidPasswordAttempts)
				{
					// Password attempts have exceeded the failure threshold. Lock out
					// the user.

					cmd.CommandText = "UPDATE `" + tableName + "` " +
									  "  SET IsLockedOut = ?IsLockedOut, LastLockedOutDate = ?LastLockedOutDate " +
									  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName";

					cmd.Parameters.Clear();

					cmd.Parameters.Add("?IsLockedOut", MySqlDbType.Bit).Value = true;
					cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = DateTime.Now;
					cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
					cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

					if (cmd.ExecuteNonQuery() < 0)
						throw new ProviderException("Unable to lock out user.");
				}
				else
				{
					// Password attempts have not exceeded the failure threshold. Update
					// the failure counts. Leave the window the same.

					switch (failureType)
					{
						case "password":
							cmd.CommandText = "UPDATE `" + tableName + "` " +
											  "  SET FailedPasswordAttemptCount = ?Count" +
											  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
							break;
						case "passwordAnswer":
							cmd.CommandText = "UPDATE `" + tableName + "` " +
											  "  SET FailedPasswordAnswerAttemptCount = ?Count" +
											  "  WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
							break;
					}
					cmd.Parameters.Clear();

					cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = failureCount;
					cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
					cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

					if (cmd.ExecuteNonQuery() < 0)
						throw new ProviderException("Unable to update failure count.");
				}
				conn.Close();
			}
		}



		//
		// CheckPassword
		//   Compares password values based on the MembershipPasswordFormat.
		//
		private bool CheckPassword(string password, string dbpassword)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.CheckPassword");

			string pass1 = password;
			string pass2 = dbpassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Encrypted:
					pass2 = UnEncodePassword(dbpassword);
					break;
				case MembershipPasswordFormat.Hashed:
					pass1 = EncodePassword(password);
					break;
				default:
					break;
			}

			return (pass1 == pass2);
		}


		//
		// EncodePassword
		//   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
		//
		private string EncodePassword(string password)
		{
			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					return password;
					break;
				case MembershipPasswordFormat.Encrypted:
					return Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					HMACSHA1 hash = new HMACSHA1();
					hash.Key = encryptionKey;
					return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
					break;
				default:
					throw new ProviderException("Unsupported password format.");
			}
		}


		//
		// UnEncodePassword
		//   Decrypts or leaves the password clear based on the PasswordFormat.
		//
		private string UnEncodePassword(string encodedPassword)
		{
			string password = encodedPassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					password =
					  Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					throw new ProviderException("Cannot unencode a hashed password.");
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return password;
		}


		//
		// MembershipProvider.FindUsersByName
		//
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.FindUsersByName");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MembershipUserCollection users = new MembershipUserCollection();

				int startIndex = pageSize * pageIndex;

				MySqlCommand cmd = new MySqlCommand("SELECT SQL_CALC_FOUND_ROWS UserId, Username, Email, PasswordQuestion," +
					" Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
					" LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
					" FROM `" + tableName + "` " +
					" WHERE Username LIKE ?UsernameSearch AND ApplicationName = ?ApplicationName " +
					" ORDER BY Username Asc" +
					" LIMIT " + startIndex + "," + pageSize, conn);
				cmd.Parameters.Add("?UsernameSearch", MySqlDbType.VarChar, 255).Value = usernameToMatch;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						users.Add(GetUserFromReader(reader));
					}
					reader.Close();
				}

				cmd.CommandText = "SELECT FOUND_ROWS()";
				totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

				conn.Close();
				return users;
			}
		}

		//
		// MembershipProvider.FindUsersByEmail
		//

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			System.Web.HttpContext.Current.Trace.Warn("Andri.Web.MySqlMembershipProvider.FindUsersByEmail");

			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				MembershipUserCollection users = new MembershipUserCollection();

				conn.Open();

				int startIndex = pageSize * pageIndex;
				MySqlCommand cmd = new MySqlCommand("SELECT SQL_CALC_FOUND_ROWS UserId, Username, Email, PasswordQuestion," +
						 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
						 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
						 " FROM `" + tableName + "` " +
						 " WHERE Email LIKE ?EmailSearch AND ApplicationName = ?ApplicationName " +
						 " ORDER BY Username Asc" +
						 " LIMIT " + startIndex + "," + pageSize, conn);
				cmd.Parameters.Add("?EmailSearch", MySqlDbType.VarChar, 255).Value = emailToMatch;
				cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						users.Add(GetUserFromReader(reader));
					}
					reader.Close();
				}

				cmd.CommandText = "SELECT FOUND_ROWS()";
				totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

				conn.Close();
				return users;
			}
		}
		
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
	}
}