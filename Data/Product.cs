namespace BlazorApp.Data;

public class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Hinhanh { get; set; }

    public decimal Price { get; set; }

    public string? Mota { get; set; }

    public string? Tentacgia { get; set; }

    public int Soluong { get; set; }

    public string? Nxb { get; set; }

    public string? Code { get; set; }

    public string? Theloai { get; set; }

    public DateTime? Ngayxb { get; set; }

    public string? Sotrang { get; set; }

    public string? Tinhtrang { get; set; }

    public string? Taiban { get; set; }

    // Parameterless constructor to prevent initialization issues
    public Product()
    { }
}
