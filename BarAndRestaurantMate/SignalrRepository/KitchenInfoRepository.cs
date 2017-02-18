using BarAndRestaurantMate.Hubs;
using BarAndRestaurantMate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.SignalrRepository
{
    public class KitchenInfoRepository
    {

        public IEnumerable<KitchenInfo> GetData()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Core"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id],[TableId],[ItemId],[Qty],[DateSold],[Cashier],[GuestOrderItemId],[Fulfilled],[Collected],[CompletedTime],[CollectedTime],[Completed],[StoreFulfilled],[StoreFulfilledTime],[SentToPOS],[Note],[IsActive] FROM [dbo].[TableItem]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            var list = reader.Cast<IDataRecord>()
                              .Select(x => new KitchenInfo()
                              {
                                  KitchenID = x.GetInt32(0)
                              }).ToList();

                            return list;
                        }
                        catch(Exception)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            KitchenHub.Show();
        }

        public IEnumerable<KitchenInfo> GetData(int guestOrderId)
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Core"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id],[ItemId],[Quantity],[Price],[Confirmed],[Delivered],[IsActive],[Paid],[CreatedDate],[GuestOrderId],[WaitreesCanSee],[GuestCanSee] FROM [dbo].[GuestOrderItem] WHERE GuestOrderId = " + guestOrderId.ToString(), connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChangeGuestOrder);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            var list = reader.Cast<IDataRecord>()
                              .Select(x => new KitchenInfo()
                              {
                                  KitchenID = x.GetInt32(0)
                              }).ToList();

                            return list;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private void dependency_OnChangeGuestOrder(object sender, SqlNotificationEventArgs e)
        {
            KitchenHub.Notify();
        }


        public IEnumerable<KitchenInfo> GetAlertData()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Core"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id],[MessageDate],[MessageDate],[Message],[IsActive],[TableName] FROM [dbo].[GuestMessage]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChangeGuestAlert);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            var list = reader.Cast<IDataRecord>()
                              .Select(x => new KitchenInfo()
                              {
                                  KitchenID = x.GetInt32(0)
                              }).ToList();

                            return list;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private void dependency_OnChangeGuestAlert(object sender, SqlNotificationEventArgs e)
        {
            KitchenHub.Alert();
        }

        public IEnumerable<KitchenInfo> GetPrinterData()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Core"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id],[TableId],[DateTime],[IsActive] FROM [dbo].[PrinterTable]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChangePrinterAlert);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            var list = reader.Cast<IDataRecord>()
                              .Select(x => new KitchenInfo()
                              {
                                  KitchenID = x.GetInt32(0)
                              }).ToList();

                            return list;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private void dependency_OnChangePrinterAlert(object sender, SqlNotificationEventArgs e)
        {
            KitchenHub.PrinterAlert();
        }
    }
}