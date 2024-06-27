using Microsoft.AspNetCore.Mvc;

namespace LaboratorioWeb.Controllers
{
    public class CancelacionPeticionController : Controller
    {

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
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
    }
}
