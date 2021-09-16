using System;
using System.IO;
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
        public ManagerController() {
            _defaultCommands = new List<string>() { "ping", "dns" };
        }

        /// <summary>
        /// Send requests to worker(s)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/manager
        ///     {
        ///         "address": "8.8.8.8",
        ///         "commands": [ "ping", "dns" ]
        ///     }
        ///
        /// </remarks>
        /// <param name="request">
        ///     Address: IP address or domain
        ///     Commands: A List of worker commands
        ///         default: [ping, dns]
        ///         supported commands:
        ///             ping
        ///             dns
        /// </param>
        [HttpPost]
        public ActionResult Post(IpFinderRequest request) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var retval = String.Empty;

            // Validate IP/domain
            if (!AddressIsValid(request.Address)) {
                return BadRequest(
                    String.Format(
                        "Invalid query address: \"{0}\"", request.Address));
            }

            var tasks = new List<Task<string>>();

            var commands =
                request.Commands.Count > 0 ? request.Commands : _defaultCommands;

            foreach (string cmd in commands) {
                tasks.Add(Task<string>.Factory.StartNew( (data) => {
                    var taskData = data as TaskData;
                    return AssignWorker(taskData.addr, taskData.cmd);
                },
                new TaskData() { addr = request.Address, cmd = cmd }
                ));
            }
            Task.WaitAll(tasks.ToArray());

            watch.Stop();

            Double elapsed_ms = watch.ElapsedMilliseconds;
            Double elapsed_s = elapsed_ms / 1000.0;
            retval = String.Format("Results in: {0}s\n", elapsed_s);

            foreach (var task in tasks) {
                var ret = task.Result;
                if (ret != null) {
                    retval = retval + "\n\n" + ret;
                }
            }

            return Ok(retval);
        }

        private string AssignWorker(string address, string command) {
            var workerAddress = Environment.GetEnvironmentVariable("IP_WORKER_ADDRESS");
            var reqAddress = workerAddress + address + "/" + command;
            var retval = String.Empty;

            HttpClient worker = new HttpClient();
            try {
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

        private static List<string> _defaultCommands;
    }
}