﻿using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Services.OrderExports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderImportController : ControllerBase
    {
        private readonly IOrderImportService _orderExportService;

        public OrderImportController(IOrderImportService orderExportService) => _orderExportService = orderExportService;

        /// <summary>
        /// Busca importação de Pedido.
        /// </summary>
        /// <param name="orderImportId">Identificação da importação de Pedidos</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpGet("{orderImportId:int}")]
        public async Task<ActionResult<OrderImportGetDto>> GetOrderExport(int orderImportId, CancellationToken cancellationToken = default)
        {
            var ordeImport = await _orderExportService.GetOrderImport(orderImportId, cancellationToken);
            return Ok(ordeImport);
        }

        /// <summary>
        /// Requere importação de Pedido.
        /// </summary>
        /// <param name="dataFile">Dados para importação</param>
        /// <param name="dataFormat">Formato da importação</param>
        /// <param name="code">Código de Confirmação</param>
        /// <param name="codeVerifier">Código de Verificação</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost("{dataFormat}")]
        public async Task<ActionResult<OrderImport>> CreateOrderExport(IFormFile dataFile, DataFormat dataFormat, Guid code, string codeVerifier, CancellationToken cancellationToken = default)
        {
            var orderImport = await _orderExportService.CreateOrderImport(dataFile, dataFormat, code, codeVerifier, cancellationToken);
            return AcceptedAtAction(nameof(GetOrderExport), new { orderImportId = orderImport.OrderImportId }, orderImport);
        }

        /// <summary>
        /// Cancela importação de Pedidos.
        /// </summary>
        /// <param name="orderImportId">Identificação da importação de Pedidos</param>
        [HttpDelete("{orderImportId:int}")]
        public IActionResult CancelOrderExport(int orderImportId)
        {
            _orderExportService.CancelOrderImport(orderImportId);
            return AcceptedAtAction(nameof(GetOrderExport), new { orderImportId });
        }
    }
}
