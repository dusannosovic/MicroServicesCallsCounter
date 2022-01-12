using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Common;
using Compute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("/HomeController/PostData")]
        public async Task<IActionResult> PostData(int container1, int container2, int container3)
        {
            var myBinding = new NetTcpBinding(SecurityMode.None);
            var myEndpoint = new EndpointAddress("net.tcp://localhost:53850/WebCommunication");

            using (var myChannelFactory = new ChannelFactory<ICompute>(myBinding, myEndpoint))
            {
                ICompute client = null;
                ViewData["Title"] = null;
                try
                {
                    client = myChannelFactory.CreateChannel();

                    if (await client.ComputeAsync(container1, container2, container3))
                    {
                        ViewData["Title"] = "Knjiga je uspesno kupljena";
                    }
                    else
                    {
                        ViewData["Title"] = "Polja nisu ispravno popunjena";
                    }
                    ((ICommunicationObject)client).Close();
                    myChannelFactory.Close();
                }
                catch
                {
                    (client as ICommunicationObject)?.Abort();
                }
            }
            return View("Index");
        }
        public IActionResult About()
        {
            List<TableEntityHtmlBinding> tableEntityHtmlBindings = new List<TableEntityHtmlBinding>();
            CloudStorageAccount _storageAccount;
            CloudTable _table;
            string a = ConfigurationManager.AppSettings["DataConnectionString"];
            _storageAccount = CloudStorageAccount.Parse(a);
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("CountTableStorage");
            var results = from g in _table.CreateQuery<CountTableEntity>() where g.PartitionKey == "CountTable" select g;
            foreach(CountTableEntity tableEntity in results.ToList())
            {
                tableEntityHtmlBindings.Add(new TableEntityHtmlBinding() { Id = tableEntity.RowKey, Count = tableEntity.NumberOfCalls });
            }
            return View(tableEntityHtmlBindings);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
