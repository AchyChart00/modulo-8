using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTest
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new ValidationContext(new {Nombre = valor});
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.AreEqual("La primera letra debe de ser mayúscula", resultado.ErrorMessage);

        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.IsNull(resultado);

        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Felipe";
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.IsNull(resultado);

        }
    }
}