namespace Sitecore.ItemWebApi
{
  using Sitecore.Diagnostics;
  using System;

  internal class BadRequestException : Exception
  {
    public BadRequestException(string message) : base(message)
    {
      Assert.ArgumentNotNull(message, "message");
    }
  }
}
