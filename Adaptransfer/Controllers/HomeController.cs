using Adaptransfer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Adaptransfer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        // Iniciando/Chamando as telas 
        public IActionResult Index() {
            return View();
        }

        public IActionResult Login() {
            return View();
        }

        //Retorna as informações do tutor para exibir na tela dashboard o nome, foto e para puaxar os valores de saída, entradas e lista de transação com base no PCD cadastrado por ele
        public IActionResult Dashboard() {
            if (HttpContext.Session.GetString("user") != null) {
                LoginTutor pegarCPF = JsonConvert.DeserializeObject<LoginTutor>(HttpContext.Session.GetString("user"));
                string id = Transacao.BuscaId(pegarCPF.CpfPCD);
                string cpfUser = pegarCPF.Cpf;
                var saidas = Transacao.Saidas(id);
                ViewBag.Saidas = saidas;
                var entradas = Transacao.Entradas(id);
                ViewBag.Entradas = entradas;
                var a = Transacao.Listar(id);
                ViewBag.Transacao = a;
                return View(LoginTutor.exibirUsuario(cpfUser));
            }
            return View("Dashboard");
        }

        //Cadastra a conta do tutor
        [HttpPost]
        public IActionResult CadastrarTutor(string nome, DateTime datanasc, string telefone, string cpf, string cpfPCD, string documentoPCD, string email, string senha, IFormFile arq) {

            LoginTutor cadastro = new LoginTutor(nome, cpf, cpfPCD, documentoPCD, email, senha, datanasc, telefone, arq);

            //Cadastra o tutor
            string resultado = cadastro.CadastroTutor();
            return View("Login");
        }

        //Faz o login
        [HttpPost]
        public IActionResult LoginConta(string nome, string telefone, string cpf, string cpfPCD, string documentoPCD, string email, string senha) {
            
            //LoginTutor login = new LoginTutor(cpf, senha);
            LoginTutor login = new LoginTutor(nome, cpf, cpfPCD, documentoPCD, email, senha, telefone);

            //Testa se o login existe , no banco de dados
            string conta = login.LogarConta(cpf, senha);
            TempData["estadoLogin"] = login.LogarConta(cpf, senha);

            LoginTutor lista = LoginTutor.ListarInfo(cpf);

            //Se o login corresponder a um cadastro, inicializando a tela dashboard, senão mantem o usuário na tela login e aparece uma mensagem informando o erro
            if (TempData["estadoLogin"] == "Logado!") {

                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(lista));
                return RedirectToAction("Dashboard");
            } else {
                ViewBag.Alert = conta;
                return View("Login");
            }
        }

        //Cadastra as despesas do usuário
        [HttpPost]
        public IActionResult CadastrarDespesa(int idConta, string nomeInstituicao, double valor, string categoria, DateTime datavenc)
        {

            Despesa cadastro = new Despesa(idConta, nomeInstituicao, valor, categoria, datavenc);

            //Cadastra o tutor
            string resultado = cadastro.CadastroDespesa();
            return RedirectToAction("Dashboard");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Finalizando a sessão e saindo da view Dashboard 
        public IActionResult Sair() {
                LoginTutor sair = JsonConvert.DeserializeObject<LoginTutor>(HttpContext.Session.GetString("user"));
                HttpContext.Session.Remove("user");
                return View("Index");
        }
    }
}
