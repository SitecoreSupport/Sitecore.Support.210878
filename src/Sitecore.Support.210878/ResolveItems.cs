namespace Sitecore.Support.ItemWebApi.Pipelines.Request
{
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Query;
  using Sitecore.Diagnostics;
  using Sitecore.Exceptions;
  using Sitecore.ItemWebApi;
  using Sitecore.ItemWebApi.Pipelines.Request;
  using Sitecore.Web;
  using System;

  public class ResolveItems : RequestProcessor
  {
    private static Item[] GetItems(RequestArgs arguments)
    {
      Item[] itemArray;
      Assert.ArgumentNotNull(arguments, "arguments");
      Database database = arguments.Context.Database;
      Item item = arguments.Context.Item;
      string queryString = WebUtil.GetQueryString("sc_database");
      if (!string.IsNullOrEmpty(queryString) && !string.Equals(queryString, database.Name, StringComparison.OrdinalIgnoreCase))
      {
        throw new AccessDeniedException($"Access to the '{queryString}' database is denied. Only members of the Sitecore Client Users role can switch databases.");
      }
      string query = WebUtil.GetQueryString("query", null);
      if (query == null)
      {
        if (item != null)
        {
          return new Item[] { item };
        }
        Logger.Warn("Context item not resolved.");
        return new Item[0];
      }
      if (query.StartsWith("fast:"))
      {
        query = query.Substring(5);
        try
        {
          return (database.SelectItems(query) ?? new Item[0]);
        }
        catch
        {
          throw new BadRequestException($"Bad Sitecore fast query: ({query}).");
        }
      }
      if (item == null)
      {
        Logger.Warn("Context item not resolved.");
        return new Item[0];
      }
      try
      {
        itemArray = Sitecore.Data.Query.Query.SelectItems(query) ?? new Item[0];
      }
      catch
      {
        throw new BadRequestException($"Bad Sitecore query ({query}).");
      }
      return itemArray;
    }

    public override void Process(RequestArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");
      arguments.Items = GetItems(arguments);
    }
  }
}