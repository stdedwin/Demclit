export class VerticalEquipo {

    idTipoVertical: String
    tipoVertical: String
    idProyecto: String
    idVertical: String;
    idEquipo:String;
    ambito: String;
    idFamilia: String;
    idClase: String;
    familia: String;
    clase: String;
    cantidad: String;
    descripcion: String;
    referencia: String;
    serial: String;
    valorMes: String;
    valorCop: String;
    valorUsd: String;
    mesCausacion: String;
    mesInicio: String;
    duracion: String;
    totalValorCop: String;
    totalPonderacionUnidad: String;
    opcionPago: String;
    nombreTipoPago: String;
    esActivo: string;
}

export class VerticalActivo
{
    idProyecto : String;
    idTipoVertical: String;
    activoVertical: String [];
}