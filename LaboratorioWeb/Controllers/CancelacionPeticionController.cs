using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LaboratorioWeb.Controllers
{
    public class CancelacionPeticionController : Controller
    {

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static ConcurrentDictionary<string, CancellationTokenSource> _tasks = new ConcurrentDictionary<string, CancellationTokenSource>();
        public IActionResult Index()
        {
            return View();
        }

        #region Peticion Cancelada sin Ajax
        [HttpGet]
        public async Task<IActionResult> Peticion(CancellationToken cancellation)
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (!cancellation.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }

                }, cancellation);
                return BadRequest("Tarea no Cancela con Existo :(");
            }
            catch (Exception ex)
            {
                return Ok("Tarea Cancela con Existo :D");
            }
        }

        [HttpGet]
        public IActionResult CancelarPeticion()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return Ok("Solicitud de cancelar correcto :D");
        }

        #endregion

        #region Peticion Cancelada con Ajax

        [HttpGet]
        public async Task<IActionResult> PeticionAjax(string taskId, CancellationToken cancellation)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            _tasks[taskId] = cancellationTokenSource;

            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                await Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        await Task.Delay(100);
                    }

                }, cancellationToken);

                _tasks.TryRemove(taskId, out _);
                return Ok("Tarea Cancela con Existo :D");
            }
            catch (Exception ex)
            {
                _tasks.TryRemove(taskId, out _);
                return Ok("Tarea Cancela con Existo :D, pero con error :D");
            }
        }

        [HttpPost]
        public IActionResult CancelarPeticionAjax(string taskId)
        {
            if (_tasks.TryRemove(taskId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
            }
            return Ok("Solicitud de cancelar correcto :D");
        }

        #endregion
    }
}
