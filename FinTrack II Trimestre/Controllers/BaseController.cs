using Microsoft.AspNetCore.Mvc;

namespace FinTrack_II_Trimestre.Controllers
{
	// Controlador base del que heredan los controladores que necesitan
	// saber cuál es el usuario logueado actualmente (ej: Category, SavingsGoal).
	// Centraliza GetCurrentUserId() para no repetir la misma lógica de sesión
	// en cada controlador.
	public abstract class BaseController : Controller
	{
		// Obtiene el UserId guardado en sesión al hacer login.
		// Devuelve 0 si no hay sesión activa (usuario no logueado).  
		protected int GetCurrentUserId()
		{
			return HttpContext.Session.GetInt32("UserId") ?? 0;
		}
	}
}