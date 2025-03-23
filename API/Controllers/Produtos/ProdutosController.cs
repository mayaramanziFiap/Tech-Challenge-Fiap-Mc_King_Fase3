using InterfaceAdapters.Produtos.Controllers.Interfaces;
using InterfaceAdapters.Produtos.Presenters.Requests;
using InterfaceAdapters.Produtos.Presenters.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Produtos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoController _produtoController;
        private readonly CognitoService _cognitoService;

        public ProdutosController(IProdutoController produtoController, CognitoService cognitoService)
        {
            _produtoController = produtoController;
            _cognitoService = cognitoService;
        }


        [HttpPost]
        public ActionResult<ProdutoResponse> Inserir(ProdutoRequest produtoRequest)
        {

            return _produtoController.Inserir(produtoRequest);
        }

        [HttpPut]
        [Route("{codigo}")]
        public ActionResult<ProdutoResponse> Alterar(int codigo, ProdutoRequest produtoRequest)
        {

            return _produtoController.Alterar(codigo, produtoRequest);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProdutoResponse>>> ConsultarAsync([FromHeader(Name = "Authorization")] string authorization,[FromQuery] ProdutoFiltroRequest produtoFiltroRequest)
        {
            // Extrai o token JWT do cabeçalho Authorization
            var token = authorization?.Replace("Bearer ", "");

            // Valida o token no Cognito
            var isValid = await _cognitoService.ValidateTokenAsync(token); 

            if (isValid)
            {
                return _produtoController.Consultar(produtoFiltroRequest);
            }
            else
            {
                return Unauthorized("Token inválido ou inexistente.");
            }
        }

       

        [HttpGet]
        [Route("{codigo}")]
        public ActionResult<ProdutoResponse> Consultar(int codigo)
        {
            return _produtoController.Consultar(codigo);
        }
    }

}
