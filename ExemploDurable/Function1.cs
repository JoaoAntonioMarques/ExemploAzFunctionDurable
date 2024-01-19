using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static class FuncaoAprovacaoDesconto
{
    [FunctionName("IniciarAprovacaoDesconto")]
    public static async Task<IActionResult> IniciarAprovacaoDesconto(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "iniciar-aprovacao-desconto")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        string corpoRequisicao = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic dados = JsonConvert.DeserializeObject(corpoRequisicao);

        string idInstancia = await starter.StartNewAsync("OrquestradorAprovacaoDesconto", idInstancia, dados);

        log.LogInformation($"Iniciada orquestração de aprovação de desconto com ID = '{idInstancia}'.");

        return new OkObjectResult($"Iniciada orquestração de aprovação de desconto com ID = '{idInstancia}'.");
    }

    [FunctionName("OrquestradorAprovacaoDesconto")]
    public static async Task ExecutarOrquestrador(
        [OrchestrationTrigger] IDurableOrchestrationContext contexto)
    {
        var dadosEvento = contexto.GetInput<dynamic>();

        //Possível lógica para verificar se o desconto é válido etc

        var statusAprovacao = await contexto.CallActivityAsync<string>("AprovarDesconto", dadosEvento);

        return new
        {
            StatusAprovacao = statusAprovacao
        };
    }

    [FunctionName("AprovarDesconto")]
    public static string AprovarDesconto([ActivityTrigger] string dadosDesconto, ILogger log)
    {
        // Lógica de aprovação aqui.        
        return "Desconto Aprovado";
    }
}
