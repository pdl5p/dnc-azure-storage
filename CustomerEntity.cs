using Microsoft.WindowsAzure.Storage.Table;

public class CustomerEntity : TableEntity
{
    public CustomerEntity(string lastName, string firstName)
    {
        this.PartitionKey = lastName;
        this.RowKey = firstName;
    }

    public CustomerEntity() { }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Twitter { get; set; }

    public string Country { get; set; }
}