﻿using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Cache;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Common
{
    public class TemplateServiceTest
    {
        private readonly Mock<IBlobStoreRepository> _blobStoreRepositoryMock = new Mock<IBlobStoreRepository>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();
        private readonly Mock<ICache> _cacheMock = new Mock<ICache>();

        private ITemplatesService _templatesService => new TemplatesService(_appParametersServiceMock.Object,
            _blobStoreRepositoryMock.Object,
            _cacheMock.Object);

        [Fact]
        public async Task ShouldGetTemplate()
        {
            string templateExpected = "<style>\r\n  * {\r\n    font-family: Arial, Helvetica, sans-serif;\r\n  }\r\n\r\n  @page {\r\n    margin: 0;\r\n  }\r\n\r\n  @media print {\r\n    @media (min-width: 83mm) and (max-width: 216mm) {\r\n      .img-container {\r\n        width: 300px;\r\n      }\r\n\r\n      .div-header {\r\n        display: flex;\r\n        flex-direction: row;\r\n        height: 120px;\r\n      }\r\n\r\n      .container-x {\r\n        width: 140mm;\r\n        height: 216mm;\r\n        margin: 0 auto;\r\n        margin-bottom: 140mm;\r\n      }\r\n\r\n      .div-logo-siste {\r\n        width: 55%;\r\n      }\r\n\r\n      .lbl-credit-value {\r\n        font-weight: bold;\r\n        font-size: 18pt;\r\n      }\r\n\r\n      .div-credit-value {\r\n        padding-top: 0px;\r\n      }\r\n\r\n      .div-payment-details {\r\n        display: flex;\r\n        flex-direction: row;\r\n      }\r\n\r\n      .personal-data {\r\n        width: 30%;\r\n        margin-left: 15%;\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .lbl-title-payment-data {\r\n        font-weight: bold;\r\n        margin-bottom: 12pt;\r\n      }\r\n\r\n      .footer {\r\n        color: white;\r\n        background-color: #666666;\r\n        text-align: center;\r\n      }\r\n\r\n      p {\r\n        margin: 1mm;\r\n      }\r\n\r\n      .divisor {\r\n        color: #666666;\r\n        height: 76px;\r\n        border: 1px dashed;\r\n        margin-left: 10px;\r\n        margin-right: 70px;\r\n      }\r\n\r\n      .divisor-details-v {\r\n        color: #666666;\r\n        height: 76px;\r\n      }\r\n\r\n      .payment-date {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .payment-warehouse-name {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .warehouse-phone {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .credit-code {\r\n        font-size: 9pt;\r\n      }\r\n\r\n      .cash-receipt {\r\n        font-size: 9pt;\r\n      }\r\n    }\r\n  }\r\n\r\n  .img-container {\r\n    width: 300px;\r\n  }\r\n\r\n  p {\r\n    margin: 1mm;\r\n  }\r\n\r\n  .div-header {\r\n    display: flex;\r\n    flex-direction: row;\r\n    height: 100px;\r\n  }\r\n\r\n  .container-x {\r\n    height: 140mm;\r\n    width: 216mm;\r\n    margin: 0 auto;\r\n    margin-bottom: 140mm;\r\n  }\r\n\r\n  .div-logo-siste {\r\n    width: 52%;\r\n  }\r\n\r\n  .print-date {\r\n    padding-bottom: 29px;\r\n    margin-right: 6%;\r\n    font-size: 8pt;\r\n    text-align: right;\r\n  }\r\n\r\n  .lbl-credit-value {\r\n    font-weight: bold;\r\n    font-size: 18pt;\r\n  }\r\n\r\n  .div-credit-value {\r\n    width: 50%;\r\n    padding-top: 2px;\r\n  }\r\n\r\n  .div-payment-details {\r\n    display: flex;\r\n    flex-direction: row;\r\n  }\r\n\r\n  .personal-data {\r\n    width: 40%;\r\n    margin-left: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .lbl-title-payment-data {\r\n    font-weight: bold;\r\n    margin-bottom: 12pt;\r\n  }\r\n\r\n  .footer {\r\n    color: white;\r\n    background-color: #666666;\r\n    text-align: center;\r\n    font-size: 9pt;\r\n    padding: 1%;\r\n  }\r\n\r\n  .divisor {\r\n    color: #666666;\r\n    height: 76px;\r\n    border: 0.5px dashed;\r\n    margin-left: 14px;\r\n    margin-right: 35px;\r\n  }\r\n\r\n  .divisor-details-v {\r\n    color: #747070;\r\n    height: 109px;\r\n    border: 1px solid;\r\n  }\r\n\r\n  .div-fill-fields {\r\n    padding-left: 15px;\r\n    margin-bottom: 20px;\r\n  }\r\n\r\n  .full-name-footer {\r\n    margin-right: 70px;\r\n  }\r\n\r\n  .type-document-footer {\r\n    padding-right: 115px;\r\n  }\r\n\r\n  .payment-date {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .payment-warehouse-name {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .purchase-warehouse-name {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .warehouse-phone {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .credit-code {\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .cash-receipt {\r\n    font-size: 9pt;\r\n    width: 46%;\r\n  }\r\n\r\n  .container-details {\r\n    display: flex;\r\n    flex-direction: row;\r\n    background-color: #e7e7e7;\r\n  }\r\n\r\n  .block-1 {\r\n    margin-top: 1%;\r\n    width: 40%;\r\n    margin-left: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .block-2 {\r\n    width: 432px;\r\n    margin-right: 2%;\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .icon-balance {\r\n    background-image: url(../../../../assets/img/saldo_credito.svg);\r\n    background-repeat: round;\r\n    background-size: cover;\r\n    width: 45px;\r\n    height: 40px;\r\n    margin-right: 5px;\r\n  }\r\n\r\n  .icon-date {\r\n    background-image: url(../../../../assets/img/dia_proximo_pago.svg);\r\n    background-repeat: round;\r\n    background-size: cover;\r\n    width: 45px;\r\n    height: 40px;\r\n    margin-right: 5px;\r\n  }\r\n\r\n  .footer-greetings {\r\n    padding-top: 15px;\r\n    text-align: center;\r\n    padding-bottom: 15px;\r\n    font-style: italic;\r\n  }\r\n\r\n  .subblock-2 {\r\n    padding-left: 10px;\r\n    padding-top: 20px;\r\n    padding-right: 10px;\r\n    display: flex;\r\n    flex-direction: row;\r\n    padding-bottom: 5px;\r\n  }\r\n\r\n  .subblock-3 {\r\n    display: flex;\r\n    flex-direction: row;\r\n    padding-left: 25px;\r\n    font-weight: bold;\r\n    font-size: 14pt;\r\n  }\r\n\r\n  .title-thanks-payment{\r\n    font-size: 12pt;\r\n  }\r\n\r\n  .text-thanks-payment{\r\n    font-size: 9pt;\r\n  }\r\n\r\n  .available-amount {\r\n    font-weight: 100;\r\n  }\r\n\r\n  .divisor-details-h {\r\n    width: 390px;\r\n    border: 1px solid black;\r\n  }\r\n\r\n  .header-detail {\r\n    line-height: 51px;\r\n    background-color: #BBBBBB;\r\n    height: 49px;\r\n    font-size: 18pt;\r\n    font-weight: 600;\r\n    color: white;\r\n    text-align: center;\r\n    margin: 0%;\r\n    margin-top: 1%;\r\n  }\r\n\r\n  .div-contact {\r\n    display: none;\r\n  }\r\n\r\n  .credit-value-tirilla {\r\n    display: none;\r\n  }\r\n\r\n  .date-tirilla {\r\n    display: none;\r\n  }\r\n\r\n  .div-info-payment{\r\n    display: flex;\r\n  }\r\n\r\n  .div-reference-code {\r\n    display: flex;\r\n    flex-direction: row;\r\n  }\r\n\r\n  @media print {\r\n    @page{\r\n    margin-top: 2%;\r\n    }\r\n    body{\r\n      margin: 0;\r\n    }\r\n    @media (max-width: 77mm) {\r\n\r\n      .container-x{\r\n        display: contents;\r\n      }\r\n\r\n      .div-logo-siste{\r\n        margin-left: 15%;\r\n        margin-bottom: 0;\r\n        height: 0;\r\n        padding-top: 1%;\r\n      }\r\n\r\n      .img-container{\r\n        width: 130%;\r\n      }\r\n\r\n      .div-header {\r\n        flex-direction: column;\r\n        text-align: center;\r\n        height: 7%;\r\n      }\r\n\r\n      .div-contact {\r\n        margin-top: 2%;\r\n        margin-bottom: 10%;\r\n        display: block;\r\n        text-align: center;\r\n        width: 100%;\r\n      }\r\n\r\n      .div-payment-details {\r\n        flex-direction: column;\r\n      }\r\n\r\n      .lbl-title-payment-data {\r\n        margin-bottom: 0;\r\n      }\r\n\r\n      .container-details {\r\n        margin-top: 2%;\r\n        flex-direction: column;\r\n        background: white;\r\n      }\r\n\r\n      .div-reference-code {\r\n        flex-direction: column;\r\n      }\r\n\r\n      .div-credit-value {\r\n        display: none;\r\n      }\r\n\r\n      .personal-data{\r\n        margin-left: 4%;\r\n        width: 100%;\r\n      }\r\n\r\n      .credit-value-tirilla {\r\n        display: block;\r\n        text-align: center;\r\n        width: 100%;\r\n        margin-top: 6%;\r\n        margin-bottom: 6%;\r\n      }\r\n\r\n      .lbl-credit-value {\r\n        margin: 0 auto;\r\n      }\r\n\r\n      .divisor {\r\n        margin-top: 2%;\r\n        margin-bottom: 2%;\r\n        margin-left: 0;\r\n        height: 0;\r\n        width: 100%;\r\n        border: none;\r\n        border-top: 3px dashed;\r\n      }\r\n\r\n      .header-detail {\r\n        line-height: 23px;\r\n        background-color: white;\r\n        height: 30px;\r\n        width: 100%;\r\n        font-size: 15px;\r\n        border-bottom: 2px solid;\r\n        border-top: 2px solid;\r\n        color: black;\r\n      }\r\n\r\n      .store-data{\r\n        margin-left: 4%;\r\n      }\r\n\r\n      .subblock-2{\r\n        margin-left: 5%;\r\n        flex-direction: column;\r\n        width: 80%;\r\n      }\r\n\r\n      .div-info-payment{\r\n        margin-bottom: 4%;\r\n      }\r\n\r\n      .block-1 {\r\n        margin-bottom: 2%;\r\n        margin-left: 3%;\r\n        width: 100%;\r\n      }\r\n\r\n      .block-2 {\r\n        width: 100%;\r\n        background: rgba(212, 212, 212, 0.986);\r\n      }\r\n\r\n      .icon-balance{\r\n        margin-right: 10%;\r\n      }\r\n\r\n      .icon-date{\r\n        margin-right: 10%;\r\n      }\r\n\r\n      .divisor-details-h {\r\n        width: 80%;\r\n      }\r\n\r\n      .divisor-details-v {\r\n        display: none;\r\n      }\r\n\r\n      .footer-greetings {\r\n        margin-top: 4%;\r\n        width: 100%;\r\n      }\r\n\r\n      .title-thanks-payment {\r\n        margin-bottom: 4%;\r\n      }\r\n\r\n      .text-thanks-payment {\r\n        margin: 0 auto;\r\n        width: 70%;\r\n      }\r\n\r\n      .date-tirilla {\r\n        margin-top: 2%;\r\n        display: block;\r\n      }\r\n\r\n      .print-date {\r\n        text-align: center;\r\n        margin-bottom: 70%;\r\n      }\r\n\r\n      .footer {\r\n        display: none;\r\n      }\r\n      .subblock-3{\r\n        margin: 0 auto;\r\n        width: 65%;\r\n        padding: 0;\r\n        text-align: center;\r\n        margin-top: 3%;\r\n        font-size: 12pt;\r\n        margin-bottom: 2%;\r\n      }\r\n    }\r\n  }\r\n</style>\r\n\r\n<div class=\"container-x\">\r\n  <div class=\"div-header\">\r\n    <div class=\"div-logo-siste\">\r\n      <img class=\"img-container\" src=\"../../../../assets/img/Impresiones_LogoSiste.png\" />\r\n    </div>\r\n    <div class=\"div-credit-value\">\r\n      <p class=\"print-date\">\r\n        Fecha de impresión: <span>1/20/2020 10:55:48 AM</span>\r\n      </p>\r\n      <p class=\"lbl-credit-value\">Valor cancelado: $13,170</p>\r\n    </div>\r\n  </div>\r\n  <div class=\"div-contact\">\r\n    <p>NIT. 854689741</p>\r\n    <p>Contáctanos: 3208899898</p>\r\n  </div>\r\n  <div class=\"div-payment-details\">\r\n    <div class=\"personal-data\">\r\n      <p class=\"lbl-title-payment-data\">\r\n        Estos son los datos de tu pago:\r\n      </p>\r\n      <p class=\"full-name\">Nombre completo: <span>SANTIAGO SERNA HIGUITA</span></p>\r\n      <p class=\"documentType\">CC: 1152686129</p>\r\n    </div>\r\n    <hr class=\"divisor\" />\r\n    <div class=\"store-data\">\r\n      <p class=\"payment-date\">Fecha del pago: <span>1/15/2020</span></p>\r\n      <p class=\"payment-warehouse-name\">\r\n        Almacén donde realizaste el pago: <span>Tienda Meli</span>\r\n      </p>\r\n      <p class=\"purchase-warehouse-name\">\r\n        Almacén donde hiciste la compra: <span>Tienda Meli</span>\r\n      </p>\r\n      <p class=\"warehouse-phone\">Teléfono almacén: <span>2585858</span></p>\r\n      <div class=\"div-reference-code\">\r\n        <p class=\"cash-receipt\">Recibo de caja: <span>444</span></p>\r\n        <p class=\"credit-code\">Código del crédito: <span>221</span></p>\r\n      </div>\r\n    </div>\r\n  </div>\r\n  <div class=\"credit-value-tirilla\">\r\n    <p class=\"lbl-credit-value\">Valor cancelado: <br>$13,170</p>\r\n  </div>\r\n  <div class=\"header-detail\">\r\n    <p>DETALLES DEL PAGO</p>\r\n  </div>\r\n  <div class=\"container-details\">\r\n    <div class=\"block-1\">\r\n      <p>Valor cancelado capital: <span>$9,600</span></p>\r\n      <p>Valor cancelado financiación: <span>$0</span></p>\r\n      <p>Valor cancelado mora: <span>$0</span></p>\r\n      <p>Valor cancelado aval: <span>$3,570</span></p>\r\n      <p>Valor cancelado cargos: <span>$0</span></p>\r\n      <p>Valor IVA aval: <span>$570</span></p>\r\n    </div>\r\n    <hr class=\"divisor-details-v\" />\r\n    <div class=\"block-2\">\r\n      <div class=\"subblock-2\">\r\n        <div class=\"div-info-payment\">\r\n          <i class=\"icon-balance\" id=\"icon-compromise\"></i>\r\n          <div style=\"font-weight: bold;\">\r\n            <p>Saldo crédito</p>\r\n            <span style=\"margin-left: 7%;\">$110,400</span>\r\n          </div>\r\n        </div>\r\n        <div class=\"div-info-payment\">\r\n          <i class=\"icon-date\" id=\"icon-compromise\"></i>\r\n          <div>\r\n            <p style=\"font-weight: bold;\">Día de tu próximo pago</p>\r\n            <p style=\"margin-left: 4%;\">2/15/2020</p>\r\n          </div>\r\n        </div>\r\n      </div>\r\n      <hr class=\"divisor-details-h\" />\r\n      <div class=\"subblock-3\">\r\n        <p>CUPO DISPONIBLE: <span class=\"available-amount\"> $45,730,620</span></p>\r\n      </div>\r\n    </div>\r\n  </div>\r\n  <div class=\"footer-greetings\">\r\n    <p class=\"title-thanks-payment\"><b>¡Gracias por tu pago!</b></p>\r\n    <p class=\"text-thanks-payment\">\r\n      Recuerda que tener tus créditos al día te permite disfrutar de tu cupo\r\n      disponible en nuestros almacenes aliados.\r\n    </p>\r\n  </div>\r\n  <div class=\"date-tirilla\">\r\n    <p class=\"print-date\">\r\n      Fecha de impresión: <span>1/20/2020 10:55:48 AM</span>\r\n    </p>\r\n  </div>\r\n  <div class=\"footer\">\r\n    Si tienes alguna duda, comunícate con nosotros al 3208899898  - <b>SISTECRÉDITO S.A.S. NIT. 854689741</b>\r\n  </div>\r\n</div>\r\n";

            byte[] fileBytes = Encoding.UTF8.GetBytes(templateExpected);

            _appParametersServiceMock.Setup(item => item.GetSettings()).Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());

            _blobStoreRepositoryMock.Setup(mock => mock.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileBytes).Verifiable();

            _cacheMock.Setup(mock => mock.GetOrCreateAsync(It.IsAny<CacheItem<string>>()))
                .Returns(async (CacheItem<string> item) =>
                {
                    return await item.GetItemAsync();
                });

            string templateActual = await _templatesService.GetAsync("TemplateName.html");

            Assert.Equal(templateExpected, templateActual);
            _blobStoreRepositoryMock.VerifyAll();
        }
    }
}