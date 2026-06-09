using System.Windows;
using CRUD.Modelos;
using MySql.Data.MySqlClient;

namespace CRUD;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnEntrar_OnClick(object sender, RoutedEventArgs e)
    {
        // Validar se esta vazio
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            MessageBox.Show("Preencha o  campo de usuário");
            txtUsuario.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtSenha.Password))
        {
            MessageBox.Show("Preencha o campo de senha");
            txtSenha.Focus();
            return;
        }

        using var conexao = new MySqlConnection(App.StringConexao);
        var query = "SELECT * FROM usuarios WHERE username = @username AND senha = @senha";

        using var comando = new MySqlCommand(query, conexao);
        comando.Parameters.AddWithValue("@username", txtUsuario.Text);
        comando.Parameters.AddWithValue("@senha", txtSenha.Password);

        try
        {
            conexao.Open();
            using var leitor = comando.ExecuteReader();
            if (!leitor.HasRows)
            {
                MessageBox.Show("Usuário ou senha incorretos.", "Erro!");
                return;
            }


            while (leitor.Read())
            {
                var usuarioBanco = new Usuario();

                usuarioBanco.Id = leitor.GetInt32(0);
                usuarioBanco.Nome = leitor.GetString(1);
                usuarioBanco.Email = leitor.GetString(2);
                usuarioBanco.Username = leitor.GetString(4);


                new MeuPerfil(usuarioBanco).Show();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void BtnCadastro_OnClick(object sender, RoutedEventArgs e)
    {
        var janelaCadastro = new Cadastro();
        Hide();
        janelaCadastro.ShowDialog();
        Show();
    }
}