using System;

namespace Azure.AD.Sso.Mobile
{
	public class Tokens
	{
		public Tokens ()
		{
		}

		public string WebApi1AccessToken {get;set;}
		public string WebApi2AccessToken {get;set;}
		public string RefreshToken {get;set;}
		public DateTimeOffset ExpiresOn {get;set;}
		public string Email {get;set;}

		public string Id { get; set; }
	}
}

