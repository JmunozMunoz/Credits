using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Helper.Test.Model;
using System.Collections.Generic;
using Xunit;

namespace Sc.Credits.Helpers.ObjectsUtils.Tests
{
    public class TemplateParameterReplaceTest
    {
        [Fact]
        public void ParametersReplaceGenericReturnTemplateWithoutParameters()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            string formatParameter = "${{{0}}}";
            string actualTemplate = "<html>Template for customer: ${IdDocument} Tabla ${table}</html>";
            string table = "<table>  <tr>    <th>Company</th>    <th>Contact</th>    <th>Country</th>  </tr>  <tr>    <td>Alfreds Futterkiste</td>    <td>Maria Anders</td>    <td>Germany</td>  </tr>  <tr>    <td>Centro comercial Moctezuma</td>    <td>Francisco Chang</td>    <td>Mexico</td>  </tr>  <tr>    <td>Ernst Handel</td>   <td>Roland Mendel</td>    <td>Austria</td>  </tr>  <tr>    <td>Island Trading</td>    <td>Helen Bennett</td>    <td>UK</td>  </tr>  <tr>    <td>Laughing Bacchus Winecellars</td>    <td>Yoshi Tannamuri</td>    <td>Canada</td>  </tr>  <tr>    <td>Magazzini Alimentari Riuniti</td>    <td>Giovanni Rovelli</td>   <td>Italy</td>  </tr></table>";
            string expectedTemplate = $"<html>Template for customer: {customer.IdDocument} Tabla {table}</html>";

            Dictionary<string, string> tables = new Dictionary<string, string>
            {
                { "${table}", table }
            };

            string templateResult = TemplateParameterReplace.ParametersReplace(actualTemplate, customer, formatParameter, tables);

            Assert.Equal(expectedTemplate, templateResult);
        }
    }
}