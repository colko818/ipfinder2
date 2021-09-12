using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Manager.Models;

namespace Manager.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase {
        [HttpPost]
        public ActionResult Post(IpFinderRequest request) {
            var retval = String.Empty;

            // Validate IP/domain
            if (!AddressIsValid(request.Address)) {
                return BadRequest(
                    String.Format(
                        "Invalid query address: \"{0}\"", request.Address));
            }

            var tasks = new List<Task<string>>();

            foreach (string cmd in request.Commands) {
                tasks.Add(Task<string>.Factory.StartNew( (data) => {
                    var taskData = data as TaskData;
                    return AssignWorker(taskData.addr, taskData.cmd);
                },
                new TaskData() { addr = request.Address, cmd = cmd }
                ));
            }
            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks) {
                var ret = task.Result;
                if (ret != null) {
                    retval = retval + "\n"+ ret;
                }
            }

            return Ok(retval);
        }

        private string AssignWorker(string address, string command) {
            var workerAddress = "http://127.0.0.1:2112/";
            var reqAddress = workerAddress + address + "/" + command;
            var retval = String.Empty;

            HttpClient worker = new HttpClient();
            try {
                Console.WriteLine(reqAddress);
                var res = worker.GetAsync(reqAddress).Result;
                res.EnsureSuccessStatusCode();

                retval = res.Content.ReadAsStringAsync().Result;
            } catch (Exception e) {
                retval = "Error: " + e.HResult.ToString("X") + "\nMessage: " + e.Message;
            }

            worker.Dispose();
            return retval;
        }

        private bool AddressIsValid(string addr) {
            return true;
        }
    }
}