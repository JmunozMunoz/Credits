using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    public enum message
    {
        RequestCredit,
        RequestPayment,
        RejectCredit,
        RejectPayment,
        ApprovalCredit,
        ApprovalPayment
    }
    public static class CancellationMessages
    {
        public static string GetMessage(this message me, string storeName, string creditNumber)
        {
            switch (me)
            {
                case message.RequestCredit:
                    return $"Tu solicitud de anulación para el crédito {creditNumber} realizado en el almacén {storeName} fue enviada con éxito el día {DateTime.Now}.";
                case message.RequestPayment:
                    return $"Tu solicitud de anulación de recibo para el crédito {creditNumber} realizado en el almacén {storeName} fue enviada con éxito el día {DateTime.Now}.";
                case message.RejectCredit:
                    return $"Tu crédito { creditNumber} realizado en el almacén { storeName} no fue anulado.Tu solicitud se rechazó el día { DateTime.Now}. Para mayor información debes dirigirte o comunicarte con el almacén {storeName}.";
                case message.RejectPayment:
                    return $"Los recibos con solicitud de anulación de tu crédito {creditNumber} realizado en el almacén {storeName} no fueron anulados. Tu solicitud fue rechazada el día {DateTime.Now}. Para mayor información debes dirigirte o comunicarte con el almacén {storeName}.";
                case message.ApprovalCredit:
                    return $"Tu crédito {creditNumber} realizado en el almacén {storeName} fue anulado exitosamente el dia {DateTime.Now}.";
                case message.ApprovalPayment:
                    return $"Los recibos para solicitud de anulación de tu crédito {creditNumber} realizado en el almacén {storeName} fueron anulados exitosamente el día {DateTime.Now}.";
                default:
                    return "";
            }
        }
    }
}
