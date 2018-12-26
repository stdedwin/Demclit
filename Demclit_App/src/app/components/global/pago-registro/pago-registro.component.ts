import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';
import { Pago } from '../../../models/pago';
import { ListaGenerica } from '../../../models/lista-generica';
import { WebapiService } from '../../../services/webapi.service';
import {MaxLength, IsCharValid, FormatValue, Modal} from '../../../app.core.functions';
declare var $:any;
declare var M:any;

@Component({
  selector: 'app-pago-registro',
  templateUrl: './pago-registro.component.html',
  styleUrls: ['./pago-registro.component.css']
})

export class PagoRegistroComponent implements OnInit {
  private sub:any;
  private userLogin:string;
  pagoAplicado:boolean = false;
  pagoEnProceso:boolean = false;
  private pagoId:number = 0;
  private listaFranquicias: ListaGenerica[];
  private modal:Modal;
  private msgExcepcionGeneral:string = "Error: No se pudo establecer conexión con el servicio. Por favor intente de nuevo y si el problema persiste escale el caso por la mesa de ayuda";


  constructor(private route: ActivatedRoute, private webapi: WebapiService) {
    this.modal = new Modal();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
       this.userLogin = params['userLogin'];
    });
    this.webapi.getData("ListaGenerica/franquicias").subscribe(
      (resp: ListaGenerica[])=>  {      this.franquiciasLoad(resp);    }
      ,error =>  {
        this.modal.open(this.msgExcepcionGeneral, "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }

  franquiciasLoad(data:ListaGenerica[]){
    this.listaFranquicias = data;
    let franquiciasValores = {};
    var options = this.listaFranquicias.map((x, i)=>{
      var option = `<option value='${x.Id}'>${x.Valor}</option>`
      return option;
    });
    $("#idFranquicia").append(options.join("")).formSelect();
  }

  resetForm(form?: NgForm) {
    if (form != null){
      form.reset();
      this.pagoAplicado = false;
      this.pagoId = 0;
      this.nextTab(false);
      $("#idFranquicia").val("").formSelect();
      $("#codCliente").focus();
    }
  }

  ngAfterViewInit() {
    $(document).ready(function(){
      $('#tabs-pago-registro').tabs({swipeable: false});
      $('li.indicator').addClass("light-blue accent-3");
      $('#codCliente, #numAutorizacion, #numTarjeta').characterCounter();
    });
  }

  preventDefault(e:any){
    e.preventDefault();
  }

  nextTab(next:boolean = true) {
      $("#tabs-pago-registro").tabs('select', (next) ? 'tab2':'tab1');
  }

  formatValue(e:any){
    let tag = (e.target || e.srcElement);
    let value = tag.value;
    value = Number(value.replace(/[^0-9.]/gi, ""));
    let maxValue = 4 * Math.pow(10, 6);
    if(value > maxValue){
      this.modal.open("Error: El monto especificado supera el límite permitido ($4' millones)", "Alerta", Modal.Type.warning);
      tag.value = "";
      return false;
    }
    else{
      return FormatValue(e, "money");
    }
  }

  isCharValid(e:any, type: string){
    return IsCharValid(e, type);
  }

  maxLength(e:any){
    return MaxLength(e);
  }

  enviarEComprobante(){
    this.webapi.sendData("EnviarEComprobante", {"IdPago": this.pagoId})
    .subscribe(
      resp =>{
        this.modal.open(resp.Msg, "Resultado de Transacción", Modal.Type.success);
      }
      ,error=>{
        this.modal.open(this.msgExcepcionGeneral, "Resultado de Transacción", Modal.Type.warning);
      }
    );
  }

  imprimirComprobante(){
    this.webapi.getData("GenerarEComprobante/" + this.pagoId)
    .subscribe(
      resp =>{
        let $eComprobante = $(resp.Data)[7];
        $eComprobante = $($eComprobante);
        $eComprobante.find("table#tab-legal").empty();
        $("#wrapper-eComprobante").html($eComprobante);//.removeClass("hide");
        //$("#wrapper-pagoForm").hide();
        let msg = resp.State ? "Comprobante generado correctamente": resp.Msg;
        alert(msg);
        setTimeout(window.print, 500);
      }
      ,error=>{
        this.modal.open(this.msgExcepcionGeneral, "Resultado de Transacción", Modal.Type.warning);
      }
    );
  }

  OnSubmit(form: NgForm) {
    if(form.valid)
    {
      let  valores = form.value;
      valores.NumAutorizacion = valores.NumAutorizacion.toUpperCase();
      valores.EMail = valores.EMail.toLowerCase();
      valores.Monto = valores.Monto.replace(/,/g,"");
      valores.UserLogin = this.userLogin;
      let  franquicia = $("#idFranquicia > option:selected").text();
      let msgConfirm = `¿Está seguro de aplicar el pago bajo la franquicia ${franquicia} ?`;
      if (confirm(msgConfirm))
      {
        this.pagoEnProceso = true;
        this.webapi.sendData("PagoRegistro", valores)
        .subscribe(
          (resp) =>{
            this.pagoEnProceso = false;
            this.pagoAplicado = resp.State;
            this.pagoId = resp.Data;
            this.modal.open(resp.Msg, "Resultado de Transacción", resp.State ? Modal.Type.success: Modal.Type.warning);
          }
          ,error => {
            this.pagoEnProceso = false;
            this.modal.open(this.msgExcepcionGeneral, "Resultado de Transacción", Modal.Type.warning);
          }
        )
      }
    }
  }

  ngOnDestroy() {
      this.sub.unsubscribe();
  }
}
