using System.Windows;
using System.Windows.Controls;
using CRUD.Modelos;
using MySql.Data.MySqlClient;

namespace CRUD;

public partial class Feed : Window
{
    private Usuario _usuario;

    public Feed(Usuario usuario)
    {
        _usuario = usuario;
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

            using var leitor = command.ExecuteReader();

            if (!leitor.HasRows)
            {
                MessageBox.Show("Nenhuma postagem foi encontrada.");
                return;
            }

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

    private void BtnCurtir_OnClick(object sender, RoutedEventArgs e)
    {
        var botao = (Button)sender;
        var postagem = (Postagem)botao.Tag;
        var query = "SELECT 1 FROM curtidas_postagens WHERE usuario_id = @usuario AND postagem_id = @postagem";
        

        using var conexao = new MySqlConnection(App.StringConexao);

        using var comando = new MySqlCommand(query, conexao);

        comando.Parameters.AddWithValue("@usuario", _usuario.Id);
        comando.Parameters.AddWithValue("@postagens", postagem.id);
        

        try
        {
            conexao.Open();
            var leitor = comando.ExecuteReader();
            string acao;

            if (leitor.HasRows)
            {
                query = "DELETE FROM curtidas_postagens WHERE usuario_id = @usuario AND postagem_id = @postagem";
                acao = "descurtir";
            }
            else
            {
                query = "INSERT INTO curtidas_postagens(usuario_id, postagem_id) VALUES(@usuario, @postagens)";
                acao = "curtir";
            }
            
            conexao.Close();
            comando.CommandText = query;
            conexao.Open();
            var linhasAfetadas = comando.ExecuteNonQuery();
            if (linhasAfetadas == 0) throw new Exception("Erro ao curtir postagem!");
        }
        catch (Exception excecao)
        {
            MessageBox.Show(excecao.Message);
        }
    }
}