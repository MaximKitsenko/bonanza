using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bonanza.Contracts.Commands;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bonanza.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private FakeBus _bus;
		private ReadModelFacade _readmodel;

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;

			_bus = new FakeBus();
			_readmodel = new ReadModelFacade();
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = rng.Next(-20, 55),
				Summary = Summaries[rng.Next(Summaries.Length)]
			})
			.ToArray();
		}

		public List<InventoryItemListDto> Index()
		{
			var r = _readmodel.GetInventoryItems();

			return r.ToList();
		}

		public InventoryItemDetailsDto Details(Guid id)
		{
			var r = _readmodel.GetInventoryItemDetails(id);
			return r;
		}

		[HttpPost]
		public ActionResult Add(string name)
		{
			_bus.Send(new CreateTenant(new TenantName(name), new TenantId(1)));

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult ChangeName(Guid id, string name, int version)
		{
			//var command = new RenameTenant(new TenantName(name), new TenantId(id), version);
			var command = new RenameTenant(new TenantName(name), new TenantId(1), SysInfo.CreateSysInfo(TenantId.CreateSystemId()), version);
			_bus.Send(command);

			return RedirectToAction("Index");
		}
	}
}
