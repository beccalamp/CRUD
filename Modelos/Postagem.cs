namespace CRUD.Modelos;

public class Postagem
{
    public int id { get; set; }
    public string conteudo { get; set; }
    public int Curtidas { get; set; }
    public DateTime Postado_em { get; set; }
    public Usuario Usuario { get; set; }
}