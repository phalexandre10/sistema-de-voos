using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservaPassagens.Data;
using ReservaPassagens.DTOs;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;

namespace ReservaPassagens.Serviço
{
    public interface IServicoCheckIn
    {
        Task<ResultadoCheckIn> RealizarCheckInOnlineAsync(RequisicaoCheckInOnline requisicao);
        Task<ResultadoCheckIn> RealizarCheckInPresencialAsync(RequisicaoCheckInPresencial requisicao);
    }
}