namespace judotech.core.tests;

public class HashPasswordTests
{
    [Fact]
    public void HashPassword_ReturnsNonEmptyValue()
    {
        var hash = DbLogin.HashPassword("correct horse battery staple");

        Assert.False(string.IsNullOrWhiteSpace(hash));
    }

    [Fact]
    public void HashPassword_IsDeterministic()
    {
        var first = DbLogin.HashPassword("same-input");
        var second = DbLogin.HashPassword("same-input");

        Assert.Equal(first, second);
    }

    [Fact]
    public void HashPassword_DiffersForDifferentInput()
    {
        var a = DbLogin.HashPassword("password-a");
        var b = DbLogin.HashPassword("password-b");

        Assert.NotEqual(a, b);
    }
}
