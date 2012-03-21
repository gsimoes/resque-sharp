using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResqueSharp.Models;
using ResqueSharp.Web.Models;

namespace ResqueSharp.Web.Controllers
{
    public class ResqueController : ResqueSharpController
    {
        private static int DefaultPageSize = 5;

        public ActionResult Overview()
        {
            var model = new QueueOverviewViewModel {

                QueuesViewModel = new QueuesViewModel {
                    Queues = ResqueClient.Queues(),
                    FailedCount = ResqueClient.FailureManager.Count()
                },

                WorkersViewModel = new WorkersViewModel {
                    Workers = ResqueClient.WorkersRunningJobs(),
                    TotalWorkers = ResqueClient.WorkersCount()
                }
            };

            return View(model);
        }

        public ActionResult Failed(int? page, int? pageSize)
        {
            var fromIndex = (page ?? 0) * (pageSize ?? DefaultPageSize);
            var count = ResqueClient.FailureManager.Count();
            var failures = ResqueClient.FailureManager.All(fromIndex, (pageSize ?? DefaultPageSize));
            var toIndex = fromIndex + failures.Count();

            ViewBag.From = fromIndex;
            ViewBag.To = toIndex;
            ViewBag.Count = count;
            ViewBag.HasNextPage = count > toIndex;
            ViewBag.NextPage = (page ?? 0) + 1;
            ViewBag.HasPreviousPage = page > 0;
            ViewBag.PreviousPage = (page ?? 0) == 0 ? 0 : page - 1;

            return View(failures);
        }

        public ActionResult Queues()
        {
            var model = new QueuesViewModel { Queues = ResqueClient.Queues(), FailedCount = ResqueClient.FailureManager.Count() };

            return View(model);
        }

        public ActionResult Working()
        {
            var model = new WorkersViewModel {
                Workers = ResqueClient.WorkersRunningJobs(),
                TotalWorkers = ResqueClient.WorkersCount()
            };

            return View(model);
        }
    }
}
