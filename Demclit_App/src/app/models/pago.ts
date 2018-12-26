export class Pago {
  
  fechaRegistro:  Date;
  idUserRegistro: string; //NH User
  idCanal: number; //Id grupo canal de venta
  canal: string; //nombre del canal
  codVendedor: number;
  codOficina: number;
  codCliente: number;
  monto: number;// valor a pagar => valisar decimales
  idFranquicia: number;
  franquicia: string;
  numAutorizacion: string; //numero de baucher 
  numTarjeta: string;
  eMail: string;
  idPago:  number;
  
}
