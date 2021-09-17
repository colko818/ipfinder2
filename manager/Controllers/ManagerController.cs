using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Manager.Models;

namespace Manager.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase {
        public ManagerController() {
            _defaultCommands = new List<string>() { "ping", "dns", "rdap", "geoip" };
        }

        // TODO: Jsonify response
        /// <summary>
        /// Send requests to worker(s)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Manager/request
        ///     {
        ///         "address": "8.8.8.8",
        ///         "commands": [ "ping", "dns", "rdap", "geoip" ]
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route("request")]
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
            var workerHost = Environment.GetEnvironmentVariable(
                "WORKER_NODE_SERVICE_SERVICE_HOST");
            var workerPort = Environment.GetEnvironmentVariable(
                "WORKER_NODE_SERVICE_SERVICE_PORT");
            var reqAddress = String.Format(
                "http://{0}:{1}/{2}/{3}",
                workerHost,
                workerPort,
                address,
                command);

            var retval = String.Empty;

            HttpClient worker = new HttpClient();
            try {
                var res = worker.GetAsync(reqAddress).Result;
                res.EnsureSuccessStatusCode();

                retval = res.Content.ReadAsStringAsync().Result;
            } catch (Exception e) {
                retval = String.Format(
                    "Error: {0} \nMessage: {1}",
                    e.HResult.ToString("X"),
                    e.Message);
            }

            worker.Dispose();
            return retval;
        }

        // TODO: Add validator that returns true only if `addr` is a well
        // formated IP address or a domain name.
        private bool AddressIsValid(string addr) {
            return true;
        }

        private static List<string> _defaultCommands;
    }
}