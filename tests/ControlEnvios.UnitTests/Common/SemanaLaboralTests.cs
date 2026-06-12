using ControlEnvios.Application.Common;

namespace ControlEnvios.UnitTests.Common;

public class SemanaLaboralTests
{
    [Theory]
    // Semana del 8 (lunes) al 14 (domingo) de junio de 2026.
    [InlineData(2026, 6, 8)]   // lunes
    [InlineData(2026, 6, 11)]  // jueves
    [InlineData(2026, 6, 14)]  // domingo (el caso que el legacy calculaba mal)
    public void Devuelve_lunes_a_domingo_de_la_misma_semana(int y, int m, int d)
    {
        var semana = SemanaLaboral.DeLaFecha(new DateOnly(y, m, d));

        Assert.Equal(new DateOnly(2026, 6, 8), semana.Inicio);
        Assert.Equal(new DateOnly(2026, 6, 14), semana.Fin);
        Assert.Equal(DayOfWeek.Monday, semana.Inicio.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, semana.Fin.DayOfWeek);
    }

    [Fact]
    public void El_domingo_pertenece_a_la_semana_que_termina_no_a_la_siguiente()
    {
        // Regresión del bug legacy: en domingo no debe saltar a la semana siguiente.
        var domingo = new DateOnly(2026, 6, 14);
        var semana = SemanaLaboral.DeLaFecha(domingo);

        Assert.Equal(new DateOnly(2026, 6, 8), semana.Inicio);
        Assert.True(domingo >= semana.Inicio && domingo <= semana.Fin);
    }
}
