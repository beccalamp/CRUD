using System.Windows;
using CRUD.Modelos;
using MySql.Data.MySqlClient;

namespace CRUD;

public partial class Feed : Window
{
    public Feed()
    {
        InitializeComponent();
        CarregarPost_QuandoIniciar();
    }

    private void CarregarPost_QuandoIniciar()
    {
        List<Postagem> ListaPostagem = [];

        const string query =
            "SELECT p.id, p.conteudo, p.curtidas, p.postado_em, u.nome,u.username FROM postagens p INNER JOIN usuarios u ON p.usuario_id = u.id";

        using var conexao = new MySqlConnection(App.StringConexao);
        using var command = new MySqlCommand(query, conexao);

        try
        {
            conexao.Open();

            // Executa o comando e guarda na variável leitor
            using var leitor = command.ExecuteReader();

            // Verifica se NÃO tem linhas
            if (!leitor.HasRows)
            {
                MessageBox.Show("Nenhuma postagem foi encontrada.");
                return;
            }

            // Se tiver, lê linha por linha na repetição
            while (leitor.Read())
            {
                var post = new Postagem
                {
                    id = leitor.GetInt32("id"),
                    conteudo = leitor.GetString("conteudo"),
                    Curtidas = leitor.GetInt32("curtidas"),
                    Postado_em = leitor.GetDateTime("postado_em"),
                    Usuario = new Usuario
                    {
                    Nome = leitor.GetString("Nome"),
                    Username = leitor.GetString("Username")
                }
                    
                };

                ListaPostagem.Add(post);
            }
            
            ItemsControlFeed.ItemsSource = ListaPostagem;
        }
        catch (MySqlException ex)
        {
            MessageBox.Show($"Erro no banco: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}