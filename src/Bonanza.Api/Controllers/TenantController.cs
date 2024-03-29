﻿using System;
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
	public class TenantController : ControllerBase
	{
		private ICommandSender _bus;
		private IReadModelFacade _readModel;

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<TenantController> _logger;

		public TenantController(ILogger<TenantController> logger, ICommandSender commandSender, IReadModelFacade readModelFacade)
		{
			_logger = logger;

			_bus = commandSender;
			_readModel = readModelFacade;
		}

		[HttpGet]
		public List<TenantListDto> Get()
		{
			var r = _readModel.GetTenants();

			return r.ToList();
		}

		public List<TenantListDto> Index()
		{
			var r = _readModel.GetTenants();

			return r.ToList();
		}

		public InventoryItemDetailsDto Details(Guid id)
		{
			var r = _readModel.GetInventoryItemDetails(id);
			return r;
		}

		[HttpPost]
		public int Add(string name)
		{
			_bus.Send(new CreateTenant(new TenantName(name), new TenantId(1)));

			return 1;
		}

		[HttpPost]
		[Route("Create")]
		public string Create([FromBody] CreateTenantRequest createTenantRequest)
		{
			_bus.Send(new CreateTenant(new TenantName(createTenantRequest.Name), new TenantId(1)));
			return createTenantRequest.Name;
		}

		[HttpPost]
		public ActionResult ChangeName(Guid id, string name, int version)
		{
			var command = new RenameTenant(new TenantName(name), new TenantId(1), SysInfo.CreateSysInfo(TenantId.CreateSystemId()) , version);
			_bus.Send(command);

			return RedirectToAction("Index");
		}
	}

	public class CreateTenantRequest
	{
		public string Name;
	}
}
