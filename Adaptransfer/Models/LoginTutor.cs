using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Adaptransfer.Models
{
    public class LoginTutor
    {
        //Fazendo a conexão com o banco de dados
        const string stringConexao = "Server=ESN509VMYSQL;Database=db_tccadaptransfer;User=aluno;Password=Senai1234";
        static MySqlConnection conecta = new MySqlConnection(stringConexao); //Variavel que faz a conexão com o banco
        static MySqlCommand query; //Variavel que faz os comandos

        //Atributos
        private string nome, cpf, cpfPCD, documentoPCD, email, senha, telefone;
        private byte[] bytesArquivo;
        private DateTime datanasc;
        private IFormFile arq;

        //Construtor do método de exibirUsuario
        public LoginTutor(string cpf, string cpfPCD, byte[] bytesArquivo, string nome)
        {
            this.nome = nome;
            this.bytesArquivo = bytesArquivo;
            this.cpf = cpf;
            this.cpfPCD = cpfPCD;
        }

        public LoginTutor(IFormFile arq) {
            this.arq = arq;
        }

        //Construtor do método de logarConta
        public LoginTutor(string cpf, string senha) {
            this.cpf = cpf;
            this.senha = senha;
        }
        //Construtor pro listarInfo
        public LoginTutor(string nome, string cpf, string cpfPCD, string documentoPCD, string email, string senha, string telefone) {
            this.nome = nome;
            this.cpf = cpf;
            this.cpfPCD = cpfPCD;
            this.documentoPCD = documentoPCD;
            this.email = email;
            this.senha = senha;
            this.telefone = telefone;
        }
        //Construtor vazio
        public LoginTutor() {
        }

        //Construtor geral
        public LoginTutor(string nome, string cpf, string cpfPCD, string documentoPCD, string email, string senha, DateTime datanasc, string telefone, IFormFile arq) {
            this.nome = nome;
            this.cpf = cpf;
            this.cpfPCD = cpfPCD;
            this.documentoPCD = documentoPCD;
            this.email = email;
            this.senha = senha;
            this.datanasc = datanasc;
            this.telefone = telefone;
            this.arq = arq;
        }

       

        //Método de GET e SET de cada dado 
        public string Nome { get => nome; set => nome = value; }
        public string Cpf { get => cpf; set => cpf = value; }
        public string CpfPCD { get => cpfPCD; set => cpfPCD = value; }
        public string DocumentoPCD { get => documentoPCD; set => documentoPCD = value; }
        public string Email { get => email; set => email = value; }
        public string Senha { get => senha; set => senha = value; }
        public DateTime Datanasc { get => datanasc; set => datanasc = value; }
        public string Telefone { get => telefone; set => telefone = value; }
        public IFormFile Arq { get => arq; set => arq = value; }
        public byte[] BytesArquivo { get => bytesArquivo; set => bytesArquivo = value; }


        //Metodo que cadastra o tutor
        public string CadastroTutor() {
            String tipoArquivo = arq.ContentType;

            if (tipoArquivo.Contains("image")) //Vê se o arquivo enviado é uma imagem
                       {
                {//se for imagem eu vou gravar no banco
                    MemoryStream s = new MemoryStream();
                    arq.CopyTo(s);
                    bytesArquivo = s.ToArray(); //Transforma o arquivo em uma sequencia de bytes para enviar ao banco
                    try {

                        conecta.Open();
                        query = new MySqlCommand("INSERT into tb_tutor(cpf_tutor, FK_USUARIO_cpf_usuario,nome,email,senha,telefone,data_nasc,documento_pcd, foto) " +
                        "VALUES (@cpf_tutor,@FK_USUARIO_cpf_usuario,@nome,@email,@senha,@telefone,@data_nasc,@documento_pcd, @foto)", conecta); //Define o comando SQL
                        //Parameters para evitar SQLInjection
                        query.Parameters.AddWithValue("@cpf_tutor", cpf);
                        query.Parameters.AddWithValue("@FK_USUARIO_cpf_usuario", cpfPCD);
                        query.Parameters.AddWithValue("@nome", nome);
                        query.Parameters.AddWithValue("@email", email);
                        query.Parameters.AddWithValue("@senha", senha);
                        query.Parameters.AddWithValue("@telefone", telefone);
                        query.Parameters.AddWithValue("@data_nasc", datanasc);
                        query.Parameters.AddWithValue("@documento_pcd", documentoPCD);
                        query.Parameters.AddWithValue("@foto", bytesArquivo);
                        query.ExecuteNonQuery(); //Executa o comando

                        Task task = Task.Delay(5000);

                        try
                        {
                            query = new MySqlCommand("UPDATE tb_conta set FK_TUTOR_cpf_tutor=@cpf_tutor where FK_USUARIO_cpf_usuario = @FK_USUARIO_cpf_usuario", conecta);
                            query.Parameters.AddWithValue("@cpf_tutor", cpf);
                            query.Parameters.AddWithValue("@FK_USUARIO_cpf_usuario", cpfPCD);
                            query.ExecuteNonQuery(); //Executa o comando
                            return "Sucesso";
                        } catch (MySqlException e)
                        {
                            return "Erro" + e.ToString();
                        }
                       
                        
                    } catch (MySqlException f) //Em caso de erro, retorna o erro
                                         {
                        return "Erro" + f.ToString();
                    }
                    finally
                    {
                            conecta.Close(); //Fecha a conexão independente do caso
                    }

                }

            } else //Se não for uma imagem, retorna uma mensagem
                         {
                return "Imagem não encontrada";
            }



        }

        //Metodo de login
        public string LogarConta(string login, string senha) {
            LoginTutor conta = new LoginTutor();
            string resultado = "";
            try {
                conecta.Open(); // Abre a conexão
                query = new MySqlCommand("SELECT * from tb_tutor where cpf_tutor=@cpf_tutor", conecta); //Define o comando sql
                //Parametros para evitar SQLInjection
                query.Parameters.AddWithValue("@cpf_tutor", login.Trim().ToString());
                query.Parameters.AddWithValue("@senha", senha.Trim().ToString());

                MySqlDataReader leitor = query.ExecuteReader();//Lê o resultado do Query ao banco

                //Se o CPF informado não for encontrado retorna a seguinte mensagem
                resultado = "Usuario não encontrado";

                //Se o CPF informado for encontrado e a senha estiver correta, loga o usuário no sistema 
                while (leitor.Read()) {
                    if(leitor["senha"].ToString() == senha) {
                        resultado = "Logado!";
                        break;
                    } else  if (leitor["foto"].ToString() != "") {
                        conta = new LoginTutor(leitor["cpf_tutor"].ToString(), leitor["FK_USUARIO_cpf_usuario"].ToString(), (byte[])leitor["foto"], leitor["nome"].ToString());
                    } else 
                    {//Se a senha informada não for correta retorna a seguinte mensagem
                        resultado = "CPF ou senha incorreta";
                    }
                }

            } catch (MySqlException f) //Pega os erros e os retorna como mensagem
                         {
                return f.ToString();
            } finally {
                conecta.Close(); //Fecha a conexão
            }
            return resultado;
        }

        //Metodo de listar todas as informações do usuário (serão úteis nos métodos do model transação)
        public static LoginTutor ListarInfo(string cpf) {
            LoginTutor lista = new LoginTutor();

            try {
                conecta.Open();
                query = new MySqlCommand("SELECT * from tb_tutor where cpf_tutor=@cpf_tutor", conecta); 
                                                                                                                                   
                query.Parameters.AddWithValue("@cpf_tutor", cpf);
                query.ExecuteNonQuery();
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) {
                  
                 lista = new LoginTutor(leitor["nome"].ToString(), leitor["cpf_tutor"].ToString(), leitor["FK_USUARIO_cpf_usuario"].ToString(), leitor["documento_pcd"].ToString(), leitor["email"].ToString(), leitor["senha"].ToString(),  leitor["telefone"].ToString());
                }

            } catch (MySqlException f){
               
            } finally {
                conecta.Close(); //Fecha a conexão independente do caso
            }

            return lista;
        }


        //Método que pega as informações do usuário e loga no dashboard correspondente com as informações dele
        static public LoginTutor exibirUsuario(string cpf)
        {
            LoginTutor conta = new LoginTutor();

            try
            {
                conecta.Open();

                query = new MySqlCommand("SELECT * FROM tb_tutor where cpf_tutor=@cpf_tutor", conecta);

                query.Parameters.AddWithValue("cpf_tutor", cpf);
                MySqlDataReader reader = query.ExecuteReader();

                while (reader.Read()) {
                    
                        if (reader["foto"].ToString() != "") {
                            conta = new LoginTutor(reader["cpf_tutor"].ToString(), reader["FK_USUARIO_cpf_usuario"].ToString(), (byte[])reader["foto"], reader["nome"].ToString());
                        }
                    
                    }

            } catch (MySqlException f) //Pega os erros e os retorna como mensagem
                         {
            } finally {
                conecta.Close(); //Fecha a conexão
            }
            return conta;
        }


    }
}
