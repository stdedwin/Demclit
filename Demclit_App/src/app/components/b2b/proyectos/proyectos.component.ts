import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { WebapiService } from '../../../services/webapi.service';
import { ListaGenerica } from '../../../models/lista-generica';
import { Vertical } from '../../../models/vertical';
import { Proyecto } from '../../../models/proyecto';
import { VerticalEquipo, VerticalActivo } from '../../../models/vertical-equipo';
import { BodegaActivo } from '../../../models/bodega-activo';

import { NgForm, FormGroup, FormControl, Validators } from '@angular/forms';

import { MaxLength, IsCharValid, FormatValue, Modal, PagerService, transform } from '../../../app.core.functions';

import * as _ from 'underscore';


declare var $: any;
declare var M: any;

@Component({
  selector: 'app-proyectos',
  templateUrl: './proyectos.component.html',
  styleUrls: ['./proyectos.component.css']
})

export class ProyectosComponent implements OnInit {

  //Formularios
  proyectoForm;
  equipoForm;

  private modal: Modal;
  private listaGenerica: ListaGenerica[];
  private listaFamilias: ListaGenerica[];
  private listaClases: ListaGenerica[];
  private pagerService: PagerService;
  private sub: any;

  private verticales: Vertical[];
  private verticalForm = new VerticalEquipo();
  private proyecto = new Proyecto();
  private seleccionar: Seleccionar[];
  private verticalesEquipos: VerticalEquipo[];
  private bodegaActivo: BodegaActivo[];

  userLogin: string;
  idProyecto: String;
  idVertical: String;
  idEquipo: String;
  idTipoVertical: String;


  consultaEliminar: String;

  //mostrar controles y validadores
  mostrarVertical: boolean = true;
  mostrarOpexRecurrente: boolean = false;
  verticalSeleccionada: boolean = false;
  proyectoCreado: boolean = false;
  editarComponente: boolean = false;
  mostrarOpcionesProyecto: boolean = false;

  //proceso guardar
  proyectoProceso: boolean = false;
  equipoProceso: boolean = false;
  activoProceso: boolean = false;

  //activos
  activosSeleccionados: boolean = false;
  activosVerticales: string[];
  private verticalActivo: VerticalActivo;

  //paginacion tabla Verticales
  private allItemsVerticales: any[];
  pagerVerticales: any = {};
  pagedItemsVerticales: any[];

  //paginacion tabla Equipos
  private allItemsEquipos: any[];
  pagerEquipos: any = {};
  pagedItemsEquipos: any[];

  //paginacion tabla Activos
  private allItemsActivos: any[];
  pagerActivos: any = {};
  pagedItemsActivos: any[];



  constructor(private route: ActivatedRoute, private webapi: WebapiService) {
    this.pagerService = new PagerService();
    this.modal = new Modal();

    this.sub = this.route.params.subscribe(params => {
      this.userLogin = params['userLogin'];
      this.idProyecto = params['idProyecto'] == "new" ? "0" : params['idProyecto'];
    });
    this.ConsultaMetodo("verticales", "#idTipoVertical", null, true);
    this.ConsultaMetodo("segmentos", "#idGrupo", null);
    this.ConsultaMetodo("familias", "#idFamilia", null);
    this.ConsultaMetodo("clases", "#idClase", null);



  }

  //Inicio Eventos Pagina
  ngOnInit() {
    this.updateControls(this.proyecto);
    this.updateControlsEquipo(this.verticalForm);
    //if (this.idProyecto != "0") {
    //this.ConsultarProyecto("ProyectoConsulta", this.idProyecto);
    //this.ConsultarVerticales("VerticalesConsulta/", this.idProyecto);
    //}
    $('#agregarVerticalModal').modal({ endingTop: '10%' });
    //Consulta Verticales
    this.TipoPago();
    //Activos
    this.activosVerticales = new Array();

  }

  ngAfterViewInit() {
    $(document).ready(function () {
      //inicializa collapsable
      $("#proyecto-collapse").collapsible({ accordion: false });
      //inicializa las Tabs Verticales
      $('#tabs-verticales').tabs({ swipeable: false });
      var tabFirst = $('#tabs-verticales li:eq(0)').css("width");
      $('#tabs-verticales li.indicator').addClass("light-blue accent-3").css({ width: tabFirst });
      //inicializa las tabs Modal
      $('#tabs-modal').tabs({ swipeable: false });
      tabFirst = $('#tabs-modal li:eq(0)').css("width");
      $('#tabs-modal li.indicator').addClass("light-blue accent-3").css({ width: tabFirst });
      //
      $("#verticalTipoPago").formSelect();

      //$('#idGrupo').formSelect();
      //$("#verticalTipoPago").formSelect();
      $("#divBotonFlotante").floatingActionButton({ hoverEnabled: false });

    });


  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  //Fin Eventos Pagina

  //Inicio Eventos Input
  isCharValid(e: any, type: string) {
    return IsCharValid(e, type);
  }

  maxLength(e: any) {
    return MaxLength(e);
  }
  //Fin Eventos Input

  //Inicio Eventos Button
  agregarModal(idEquipo: String = null, idFamilia: String = null) {

    if ($("#idTipoVertical").val() == '-1' && idEquipo == null) {
      this.modal.open("Por favor seleccione un tipo de vertical", "Advertencia", Modal.Type.success);
      return;
    }

    //Editar Equipo
    if (idEquipo != null) {
      this.editarComponente = true;
      this.ConsultarVerticalesEquipos((this.idProyecto + "/" + this.idVertical), idEquipo, false);
      $("#tabs-modal").tabs('select', 'tab3');
      //this.ConsultaMetodo("clases", "#idClase", idFamilia);
      //M.updateTextFields();
    }
    //Nuevo Equipo
    else {
      this.activosVerticales = new Array();
      this.idEquipo = "0";
      this.editarComponente = false;
      this.equipoForm.reset();
      $("#idFamilia").val("-1").formSelect();
      $("#verticalTipoPago").val("-1").formSelect();
      $("#idClase").empty().formSelect();
      $("#idFamiliaActivo").val("-1").formSelect();
      $("#idClaseActivo").empty().formSelect();
      this.pagedItemsActivos = [];
      this.pagerActivos = {};

      //Reset Materialize

      $('#lblMesInicio').removeClass('active');
      $('#mesInicio').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblDuracionVertical').removeClass('active');
      $('#duracionVertical').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine  ');
      $('#lblMesCausacion').removeClass('active');
      $('#mesCausacion').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine ');
      $('#lblCantidad').removeClass('active');
      $('#cantidad').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblValorMes').removeClass('active');
      $('#valorMes').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblAmbito').removeClass('active');
      $('#ambito').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblDescripcion').removeClass('active');
      $('#descripcion').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblValorCop').removeClass('active');
      $('#valorCop').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#lblValorUsd').removeClass('active');
      $('#valorUsd').removeClass('valid ng-valid ng-touched ng-dirty').addClass('ng-invalid validate ng-untouched ng-pristine');
      $('#idFamilia').removeClass('ng-touched ng-dirty ng-valid').addClass('ng-untouched ng-pristine ng-invalid');
      $('#idClase').removeClass('ng-touched ng-dirty ng-valid').addClass('ng-untouched ng-pristine ng-invalid');
      $('#verticalTipoPago').removeClass('ng-touched ng-dirty ng-valid').addClass('ng-untouched ng-pristine ng-invalid');

      $("#tabs-modal").tabs('select', 'tab3');
    }


    let modalTag = $('#agregarVerticalModal');
    modalTag.modal('open').css({ "width": "90%", "max-height": "90%" });
  }
  //Fin Eventos Button

  //Inicio Eventos Select
  selectTiposVerticales() {
    this.idTipoVertical = $("#idTipoVertical > optgroup > option:selected").val();
    this.filtrarListaGenerica(this.listaFamilias, "#idFamilia", this.idTipoVertical);
    this.filtrarListaGenerica(this.listaFamilias, "#idFamiliaActivo", this.idTipoVertical);
  }

  selectFamiliaClases(activos: boolean = false) {
    this.activosVerticales = new Array();
    if (activos) {
      let familia = $("#idFamiliaActivo  > option:selected").val();
      this.filtrarListaGenerica(this.listaClases, "#idClaseActivo", familia);
    }
    else {
      let familia = $("#idFamilia  > option:selected").val();
      this.filtrarListaGenerica(this.listaClases, "#idClase", familia);
    }
  }

  mostrarTipoPago() {
    let verticalTipoPago = $("#verticalTipoPago > option:selected").val();
    this.mostrarOpexRecurrente = (verticalTipoPago == '2');
    if (verticalTipoPago == '2') {
      //$('#mesCausacion').val("");
      //OpexOneTime y Opex
      this.equipoForm.patchValue({ mesCausacion: '' });
      this.equipoForm.get('mesCausacion').clearValidators();
      this.equipoForm.get('mesCausacion').updateValueAndValidity();

      //OpexRecurrente
      this.equipoForm.get('mesInicio').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('mesInicio').updateValueAndValidity();
      this.equipoForm.get('duracionVertical').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('duracionVertical').updateValueAndValidity();


    }
    else {
      //$('#mesInicio').val("");
      //$('#duracionVertical').val("");

      //OpexRecurrente
      this.equipoForm.patchValue({ mesInicio: '', duracionVertical: '' });
      this.equipoForm.get('mesInicio').clearValidators();
      this.equipoForm.get('mesInicio').updateValueAndValidity();
      this.equipoForm.get('duracionVertical').clearValidators();
      this.equipoForm.get('duracionVertical').updateValueAndValidity();

      //OpexOneTime y Opex
      this.equipoForm.get('mesCausacion').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('mesCausacion').updateValueAndValidity();


    }
  }

  //Fin Eventos Select

  //Inicio Eventos Tab
  nextTab(next: boolean = true, idVertical: string = null, idTipoVertical: string = null) {
    $("#tabs-verticales").tabs('select', (next) ? 'tab2' : 'tab1');

    this.verticalSeleccionada = true;
    this.idVertical = idVertical;
    this.idTipoVertical = idTipoVertical;
    //this.ConsultaMetodo("familias", "#idFamilia", idTipoVertical);
    this.ConsultarVerticalesEquipos((this.idProyecto + "/" + idVertical));
  }
  onChangeTab() {
    this.verticalSeleccionada = false;
  }
  //Fin Eventos Tab

  //Inicio Metodos
  updateControls(proyecto: Proyecto) {
    this.proyectoForm = new FormGroup({
      idOportunidad: new FormControl(proyecto.idOportunidad),
      numIdentidadCliente: new FormControl(proyecto.numIdentidadCliente),
      nombreProyecto: new FormControl(proyecto.nombreProyecto),
      nombreCliente: new FormControl(proyecto.nombreCliente),
      duracion: new FormControl(proyecto.duracion, [Validators.max(255), Validators.min(1)]),
      idGrupo: new FormControl(proyecto.idGrupo)
    });
    //this.proyectoForm.controls['idGrupo'].setValue(proyecto.idGrupo, {onlySelf: true});
    M.updateTextFields();
  }

  updateControlsEquipo(equipo: VerticalEquipo) {
    
    let pValorCop = equipo.valorCop == "0" ? "" : transform(Number(equipo.valorCop));
    let pValorUsd = equipo.valorUsd == "0" ? "" : transform(Number(equipo.valorUsd));
    let pValorMes = equipo.valorMes == "0" ? "" : transform(Number(equipo.valorMes));
     // Se reemplazan los puntos para no tener problemas al acutalizar   
     pValorMes = pValorMes.replace(/\./gi,'');
     pValorUsd = pValorUsd.replace(/\./gi,'');
     pValorCop = pValorCop.replace(/\./gi,'');
    

    this.equipoForm = new FormGroup({
      idFamilia: new FormControl(equipo.idFamilia),
      idClase: new FormControl(equipo.idClase),
      cantidad: new FormControl(equipo.cantidad),
      valorMes: new FormControl(pValorMes),
      ambito: new FormControl(equipo.ambito),
      descripcion: new FormControl(equipo.descripcion),
      verticalTipoPago: new FormControl(equipo.opcionPago),
      mesCausacion: new FormControl(equipo.mesCausacion == "0" ? "" : equipo.mesCausacion, [Validators.max(255), Validators.min(1)]),
      valorCop: new FormControl(pValorCop),
      valorUsd: new FormControl(pValorUsd),
      mesInicio: new FormControl(equipo.mesInicio == "0" ? "" : equipo.mesInicio, [Validators.max(255), Validators.min(1)]),
      duracionVertical: new FormControl(equipo.duracion == "0" ? "" : equipo.duracion, [Validators.max(255), Validators.min(1)]),
    });

    if (equipo.opcionPago == "2" && equipo.mesCausacion == "0") {
      this.mostrarOpexRecurrente = true;
      //OpexOneTime y Opex
      this.equipoForm.get('mesCausacion').clearValidators();
      this.equipoForm.get('mesCausacion').updateValueAndValidity();

      //OpexRecurrente
      this.equipoForm.get('mesInicio').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('mesInicio').updateValueAndValidity();
      this.equipoForm.get('duracionVertical').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('duracionVertical').updateValueAndValidity();
    }
    else if ((equipo.opcionPago == "1" || equipo.opcionPago == "3") && equipo.mesCausacion != "0") {

      this.mostrarOpexRecurrente = false;
      //OpexRecurrente
      this.equipoForm.get('mesInicio').clearValidators();
      this.equipoForm.get('mesInicio').updateValueAndValidity();
      this.equipoForm.get('duracionVertical').clearValidators();
      this.equipoForm.get('duracionVertical').updateValueAndValidity();

      //OpexOneTime y Opex
      this.equipoForm.get('mesCausacion').setValidators([Validators.required, Validators.max(255), Validators.min(1)]);
      this.equipoForm.get('mesCausacion').updateValueAndValidity();
    }

    M.updateTextFields();
  }
  mostrarAgregarVertical() {
    let tipoVertical = $("#idTipoVertical > option:selected").val();
    this.mostrarVertical = (tipoVertical == '2');
  }
  TipoPago() {
    $("#verticalTipoPago").empty();
    var vDefault = "<option value='-1'>Seleccione</option>";
    this.seleccionar = [
      { valor: "1", name: "Opex One Time" },
      { valor: "2", name: "Opex Recurrente" },
      { valor: "3", name: "Capex" }
    ];

    var options = this.seleccionar.map((x, i) => {
      var option = `<option value='${x.valor}'>${x.name}</option>`
      return option;
    });
    $("#verticalTipoPago").append(vDefault).formSelect();
    $("#verticalTipoPago").append(options.join("")).formSelect();
  }
  ConsultarProyecto(pConsulta: String, pFiltro: String): any {
    this.webapi.getData(pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: Proyecto) => {
        this.proyecto = resp[0];
        this.updateControls(this.proyecto);

        this.idProyecto = resp[0].idProyecto;
        $("#idGrupo").val(resp[0].idGrupo).formSelect();

        $('#lblNumIdentidadCliente').addClass('active');
        $('#numIdentidadCliente').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-touched ng-dirty ng-valid');

        $('#lblNombreProyecto').addClass('active');
        $('#nombreProyecto').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-touched ng-dirty ng-valid');

        $('#lblNombreCliente').addClass('active');
        $('#nombreCliente').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-touched ng-dirty ng-valid');

        $('#lblIdOportunidad').addClass('active');
        $('#idOportunidad').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-touched ng-dirty ng-valid');

        $('#lblDuracion').addClass('active');
        $('#duracion').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-touched ng-dirty ng-valid');

        //$('#lblIdGrupo').addClass('active');
        $('#idGrupo').removeClass('ng-untouched ng-pristine ng-invalid').addClass('ng-touched ng-dirty ng-valid');
        //$('#numIdentidadCliente').val(resp[0].numIdentidadCliente);
        //$('#numIdentidadCliente').val(resp[0].numIdentidadCliente);
        //$('#nombreProyecto').val(resp[0].nombreProyecto);
        //$('#nombreCliente').val(resp[0].nombreCliente);
        //$('#duracion').val(resp[0].duracion);
        //$('#idGrupo').val(resp[0].idGrupo).formSelect();

        this.proyectoCreado = true;
        $('#proyecto-collapse').collapsible("open", "1");
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }
  ConsultarVerticales(pConsulta: String, pFiltro: String): any {
    this.verticales = {} as Vertical[];
    this.webapi.getData(pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: Vertical[]) => {
        this.mostrarOpcionesProyecto = (resp.length > 0);
        //console.log(this.mostrarOpcionesProyecto);
        this.verticales = resp;
        this.allItemsVerticales = this.verticales;
        this.setPageVerticales(1);
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }
  EliminarVertical(pIdVertical: string) {
    this.verticalForm = {} as VerticalEquipo;
    this.verticalForm.idProyecto = this.idProyecto;
    this.verticalForm.idVertical = pIdVertical;

    let msgConfirm = `¿Está seguro que desea eliminar la vertical del proyecto?`;
    if (confirm(msgConfirm)) {
      this.EliminarMetodo(this.verticalForm, "Verticales");
    }
  }
  EliminarEquipo(pIdEquipo: string, esActivo: string) {
    this.verticalForm = {} as VerticalEquipo;
    this.verticalForm.idEquipo = pIdEquipo;
    this.verticalForm.idVertical = this.idVertical;
    this.verticalForm.idProyecto = this.idProyecto;
    this.verticalForm.esActivo = esActivo;
    let msgConfirm = `¿Está seguro que desea eliminar el equipo de la vertical?`;
    if (confirm(msgConfirm)) {
      this.EliminarMetodo(this.verticalForm, "Equipos");
    }
  }
  EliminarMetodo(pVerticalEquipo: VerticalEquipo, pTipo: String): any {

    this.webapi.putData("VerticalesEquipoEliminar", pVerticalEquipo).subscribe(
      (resp: Boolean) => {
        if (pTipo == "Verticales") {
          this.ConsultarVerticales("VerticalesConsulta/", this.idProyecto);
        }
        else {
          this.ConsultarVerticalesEquipos((this.idProyecto + "/" + this.idVertical));
        }
        this.modal.open("El registro se elimino con exito", "Resultado de Transacción", resp ? Modal.Type.success : Modal.Type.warning);
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }
  ConsultarVerticalesEquipos(pConsulta: String, pFiltro: String = null, pGrilla: boolean = true): any {
    this.webapi.getData("VerticalesEquiposConsulta/" + pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: VerticalEquipo[]) => {


        if (pGrilla) {
          this.verticalesEquipos = resp;
          this.allItemsEquipos = this.verticalesEquipos;
          this.setPageEquipos(1);
        }
        else {
          this.updateControlsEquipo(resp[0]);

          this.idEquipo = resp[0].idEquipo;
          this.idVertical = resp[0].idVertical;
          this.filtrarListaGenerica(this.listaFamilias, "#idFamilia", resp[0].idTipoVertical);
          this.filtrarListaGenerica(this.listaClases, "#idClase", resp[0].idFamilia);


          $("#idFamilia").val(resp[0].idFamilia).formSelect();
          $("#idClase").val(resp[0].idClase).formSelect();
          $("#verticalTipoPago").val(resp[0].opcionPago).formSelect();

          // $("#idFamilia").formSelect();
          // $("#idClase").formSelect();
          // $("#verticalTipoPago").formSelect();

           if (resp[0].opcionPago == "2") {

          //this.mostrarOpexRecurrente = true;
          $('#lblMesInicio').addClass('active');
          $('#mesInicio').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblDuracionVertical').addClass('active');
          $('#duracionVertical').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');
          //$('#mesCausacion').val("");
          //$('#mesInicio').val(resp[0].mesInicio);
          //$('#duracionVertical').val(resp[0].duracion);

           }
           else {

          //this.mostrarOpexRecurrente = false;
          $('#lblMesCausacion').addClass('active');
          $('#mesCausacion').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');
          //$('#mesInicio').val("");
          //$('#duracionVertical').val("");
          //$('#mesCausacion').val(resp[0].mesCausacion);
           }

          $('#lblCantidad').addClass('active');
          $('#cantidad').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblValorMes').addClass('active');
          $('#valorMes').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblAmbito').addClass('active');
          $('#ambito').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblDescripcion').addClass('active');
          $('#descripcion').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblValorCop').addClass('active');
          $('#valorCop').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          $('#lblValorUsd').addClass('active');
          $('#valorUsd').removeClass('ng-invalid invalid ng-untouched ng-pristine').addClass('valid ng-valid ng-touched ng-dirty');

          //$('#lblIdFamilia').addClass('active');
          $('#idFamilia').removeClass('ng-untouched ng-pristine ng-invalid').addClass('ng-touched ng-dirty ng-valid');

          //$('#lblIdClase').addClass('active');
          $('#idClase').removeClass('ng-untouched ng-pristine ng-invalid').addClass('ng-touched ng-dirty ng-valid');

          //$('#lblVerticalTipoPago').addClass('active');
          $('#verticalTipoPago').removeClass('ng-untouched ng-pristine ng-invalid').addClass('ng-touched ng-dirty ng-valid');




          //$('#cantidad').val(resp[0].cantidad);
          //$('#valorMes').val(resp[0].valorMes);
          //$('#ambito').val(resp[0].ambito);
          //$('#descripcion').val(resp[0].descripcion);
          //$('#valorCop').val(resp[0].valorCop);
          //$('#valorUsd').val(resp[0].valorUsd);
          //$('#idFamilia').val(resp[0].idFamilia);
          //$('#idClase').val(resp[0].idClase);
          //$('#verticalTipoPago').val(resp[0].opcionPago);

        }
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }
  ConsultaMetodo(pConsulta: String, pObjeto: String, pFiltro: String = null, pConGrupo: Boolean = false) {
    this.webapi.getData("ListaGenerica/" + pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: ListaGenerica[]) => { pConGrupo ? (this.LlenarSelectGroup(resp, pObjeto)) : (this.LlenarSelect(resp, pObjeto, pConsulta)); }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }
  LlenarSelect(data: ListaGenerica[], pSelect: String, pConsulta: String) {

    this.listaGenerica = data;
    if (pConsulta == "familias") {
      this.listaFamilias = data;
      return;
    }
    else if (pConsulta == "clases") {
      this.listaClases = data;
      return;
    }

    $(pSelect).empty();
    var vDefault = "<option [ngValue]='-1'>Seleccione</option>";
    let franquiciasValores = {};
    var options = this.listaGenerica.map((x, i) => {

      var option = `<option value='${x.Id}'>${x.Valor}</option>`
      return option;
    });
    $(pSelect).append(vDefault).formSelect();
    $(pSelect).append(options.join("")).formSelect();

    if (this.idProyecto != "0" && pConsulta == 'segmentos') {

      this.ConsultarProyecto("ProyectoConsulta", this.idProyecto);
      this.ConsultarVerticales("VerticalesConsulta/", this.idProyecto);
    }
  }
  LlenarSelectGroup(data: ListaGenerica[], pSelect: String) {
    this.listaGenerica = data;
    let grupos = [];
    let grupo: string;
    let grupoOld: string;
    $(pSelect).empty();
    var vDefault = "<option value='-1'>Seleccione</option>";
    $.each(this.listaGenerica, (i, x) => {
      grupo = x.Grupo;
      let numItems = grupos.length;
      let $optGroup = grupos[numItems > 0 ? numItems - 1 : numItems];
      if (grupo !== grupoOld) {
        $optGroup = $(`<optgroup label='${grupo}'></optgroup>`);
        grupos.push($optGroup);
      }
      var option = `<option value='${x.Id}' data-group='${grupo}'>${x.Valor}</option>`;
      $optGroup.append(option);
      grupoOld = grupo;
    });
    var options = grupos;
    $(pSelect).append(vDefault).formSelect();
    $(pSelect).append(options).formSelect();
  }
  filtrarListaGenerica(data: ListaGenerica[], pSelect: String, pFiltrar: String) {
    let listaFiltrada = data.filter(u => u.Grupo == pFiltrar);
    $(pSelect).empty();
    var vDefault = "<option value='-1'>Seleccione</option>";
    var options = listaFiltrada.map((x, i) => {

      var option = `<option value='${x.Id}'>${x.Valor}</option>`
      return option;
    });
    $(pSelect).append(vDefault).formSelect();
    $(pSelect).append(options.join("")).formSelect();
  }
  setPageVerticales(page: number) {
    if (page < 1 || page > this.pagerVerticales.totalPages) {
      return;
    }
    // get pager object from service
    this.pagerVerticales = this.pagerService.getPager(this.allItemsVerticales.length, page);
    // get current page of items
    this.pagedItemsVerticales = this.allItemsVerticales.slice(this.pagerVerticales.startIndex, this.pagerVerticales.endIndex + 1);
  }
  setPageEquipos(page: number) {
    if (page < 1 || page > this.pagerEquipos.totalPages) {
      return;
    }
    // get pager object from service
    this.pagerEquipos = this.pagerService.getPager(this.allItemsEquipos.length, page);
    // get current page of items
    this.pagedItemsEquipos = this.allItemsEquipos.slice(this.pagerEquipos.startIndex, this.pagerEquipos.endIndex + 1);
  }
  setPageActivos(page: number) {
    if (page < 1 || page > this.pagerActivos.totalPages) {
      return;
    }
    // get pager object from service
    this.pagerActivos = this.pagerService.getPager(this.allItemsActivos.length, page);
    // get current page of items
    this.pagedItemsActivos = this.allItemsActivos.slice(this.pagerActivos.startIndex, this.pagerActivos.endIndex + 1);
  }
  //Activos
  ConsultaActivos() {
    this.activosVerticales = new Array();
    let letIdFamilia = $('#idFamiliaActivo').val();
    let letIdClase = $('#idClaseActivo').val();
    this.webapi.getData("BodegaActivosConsulta/" + this.idTipoVertical + "/" + letIdFamilia + "/" + letIdClase).subscribe(
      (resp: BodegaActivo[]) => {

        this.bodegaActivo = resp;
        this.allItemsActivos = this.bodegaActivo;
        this.setPageActivos(1);
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )

  }
  checkedActivos(pIdEquipo: string, pChecked: any) {

    
    //console.log(this.activosVerticales);
    if (pChecked) {
      this.activosVerticales.push(pIdEquipo);
      // console.log(this.activosVerticales);
    }
    else {
      this.activosVerticales.splice(this.activosVerticales.indexOf(pIdEquipo), 1);
      // console.log(this.activosVerticales);
    }
    this.activosSeleccionados = this.activosVerticales.length > 0;
  }

  //Fin Metodos

  //Inicio Eventos Button
  OnSubmitProyecto(form: NgForm) {
    //console.log(form.value);
    if (form.valid) {
      this.proyecto = new Proyecto;
      //let valores = form.value;
      this.proyecto.idProyecto = Number(this.idProyecto);
      this.proyecto.idOportunidad = form.value.idOportunidad;
      this.proyecto.nombreProyecto = form.value.nombreProyecto;
      this.proyecto.duracion = form.value.duracion;
      this.proyecto.numIdentidadCliente = form.value.numIdentidadCliente;
      this.proyecto.nombreCliente = form.value.nombreCliente;
      this.proyecto.idGrupo = form.value.idGrupo;
      this.proyecto.userLogin = this.userLogin;
      /*
      this.proyecto.idOportunidad = String($("#idOportunidad").val());
      this.proyecto.nombreProyecto = String($("#nombreProyecto").val());
      this.proyecto.duracion = Number($("#duracion").val());
      this.proyecto.numIdentidadCliente =Number($("#numIdentidadCliente").val());
      this.proyecto.nombreCliente = String($("#nombreCliente").val());
      this.proyecto.idGrupo = Number($("#idGrupo > option:selected").val());
      */

      let msgConfirm = this.proyectoCreado ? `¿Está seguro de actualizar el proyecto ?` :`¿Está seguro de guardar el proyecto ?`;
      if (confirm(msgConfirm)) {
        this.proyectoProceso = true;
        // console.log(this.proyecto);
        this.webapi.sendData("ProyectoRegistro", this.proyecto)
          .subscribe(
            (resp) => {

              if (resp.State) {
                this.proyectoCreado = resp.State;
                $('#proyecto-collapse').collapsible("open", "1");
                this.idProyecto = resp.Data;
                // console.log(this.idProyecto);
                this.modal.open(resp.Msg, "Resultado de Transacción", resp.State ? Modal.Type.success : Modal.Type.warning);
                this.proyectoProceso = false;
                if (this.idProyecto == "0") {
                  this.ConsultarProyecto("ProyectoConsulta", this.idProyecto);
                }

              }
              else {
                this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
                this.proyectoProceso = false;
              }


            }
            , error => {
              this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
              this.proyectoProceso = false;
            }
          )
      }
      else {
        this.proyectoProceso = false;
      }
    }
  }

  OnSubmitVertical(form: NgForm) {
    console.log(form.valid);
    if (form.valid) {
      //console.log(form.value);
      if (form.value.valorCop == null && form.value.valorUsd == null) {
        this.modal.open("Por favor ingrese un valor de costo en COP o USD", "Advertencia", Modal.Type.success);
        return;
      }

      if (form.value.idFamilia == "-1") {
        this.modal.open("Por favor seleccione una familia", "Advertencia", Modal.Type.success);
        return;
      }

      if (form.value.idClase == "-1") {
        this.modal.open("Por favor seleccione una familia", "Advertencia", Modal.Type.success);
        return;
      }

      if (form.value.verticalTipoPago == "-1") {
        this.modal.open("Por favor seleccione un tipo de costo", "Advertencia", Modal.Type.success);
        return;
      }

      this.equipoProceso = true;
      this.verticalForm = new VerticalEquipo();
      this.verticalForm.idEquipo = this.idEquipo;
      this.verticalForm.idFamilia = form.value.idFamilia;
      this.verticalForm.idClase = form.value.idClase;
      this.verticalForm.cantidad = form.value.cantidad;
      this.verticalForm.ambito = form.value.ambito;
      this.verticalForm.descripcion = form.value.descripcion;
      this.verticalForm.valorMes = form.value.valorMes.replace(/,/g, "");
      this.verticalForm.mesCausacion = form.value.mesCausacion == "" ? 0 : form.value.mesCausacion;
      this.verticalForm.valorCop = form.value.valorCop == "" ? 0 : form.value.valorCop.replace(/,/g, "");
      this.verticalForm.valorUsd = form.value.valorUsd == "" ? 0 : form.value.valorUsd.replace(/,/g, "");
      this.verticalForm.mesInicio = form.value.mesInicio == "" ? 0 : form.value.mesInicio;
      this.verticalForm.duracion = form.value.duracionVertical == "" ? 0 : form.value.duracionVertical;
      this.verticalForm.opcionPago = $('#verticalTipoPago').val();
      this.verticalForm.idProyecto = this.idProyecto;
      this.verticalForm.idTipoVertical = this.idTipoVertical;
      console.log(this.verticalForm);
      let msgConfirm = this.editarComponente ? `¿Desea actualizar la Vertical?` :`¿Desea crear la nueva vertical?`;
      if (confirm(msgConfirm)) {
        

        this.webapi.sendData("VerticalEquipoRegistro", this.verticalForm)
          .subscribe(
            (resp) => {

              if (resp.State) {
                //this.idVertical = resp.Data;
                this.modal.open(resp.Msg, "Resultado de Transacción", resp.State ? Modal.Type.success : Modal.Type.warning);
                this.equipoProceso = false;
                $('#idTipoVertical').val("-1")
                $("#idTipoVertical").formSelect();
                let modalTag = $('#agregarVerticalModal');
                modalTag.modal("close");
                $("#tabs-verticales").tabs('select', 'tab1');
                this.ConsultarVerticales("VerticalesConsulta/", this.idProyecto);
              }
              else {
                this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
                this.equipoProceso = false;
              }
            }
            , error => {
              this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
              this.equipoProceso = false;
            }
          )

      }
      else {
        this.equipoProceso = false;
      }

    }
  }

  resetForm(form?: NgForm) {
    if (form != null) {
      form.reset();
      this.proyectoCreado = false;
      this.idProyecto = "0";
      this.nextTab(false);
      $("#idOportunidad").focus();
    }
  }

  OnSubmitActivos(form: NgForm) {
    if (form.valid) {
      this.verticalActivo = new VerticalActivo;
      //let valores = form.value;

      this.verticalActivo.idProyecto = this.idProyecto;
      this.verticalActivo.idTipoVertical = this.idTipoVertical;
      this.verticalActivo.activoVertical = this.activosVerticales;

      let msgConfirm = `¿Desea asociar (el) o (los) activos al proyecto?`;
      if (confirm(msgConfirm)) {
        this.activoProceso = true;

        // console.log(this.proyecto);
        this.webapi.sendData("VerticalActivoRegistro", this.verticalActivo)
          .subscribe(
            (resp) => {
              resp.State;
              //this.idVertical = resp.Data;
              this.modal.open(resp.Msg, "Resultado de Transacción", resp.State ? Modal.Type.success : Modal.Type.warning);
              form.reset();
              $('#idTipoVertical').val("-1")
              $("#idTipoVertical").formSelect();

              this.ConsultarVerticales("VerticalesConsulta/", this.idProyecto);
              let modalTag = $('#agregarVerticalModal');
              modalTag.modal("close");
              this.activoProceso = false;


            }
            , error => {
              this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
              this.activoProceso = false;
            }
          )
      }


    }
  }

  ProyectoEstadoCambio() {
    this.proyecto = new Proyecto;
    this.proyecto.idProyecto = Number(this.idProyecto);

    let msgConfirm = `¿Desea cerrar el proyecto?`;
    if (confirm(msgConfirm)) {
      this.webapi.sendData("ProyectoEstadoCambio", this.proyecto)
        .subscribe(
          (resp) => {
            if (resp.State) {
              this.modal.open(resp.Msg, "Resultado de Transacción", resp.State ? Modal.Type.success : Modal.Type.warning);
              window.location.href = "./b2b-proyectos/consulta/"+ this.userLogin;
            }
          }
          , error => {
            this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor intente nuevamente.", "Resultado de Transacción", Modal.Type.warning);
          }
        )
    }
  }

  Regresar() {

    window.location.href = "./b2b-proyectos/consulta/" + this.userLogin;


  }

  Descargar() {

    this.webapi.getFile("GenerarReporteFinanciero/" + this.idProyecto);


  }
  //Fin Eventos Button

  //Inicio Eventos TextBox
  keyPressCopUsd(pControl: String) {
    // console.log(pControl);
    if (pControl == "COP" && $("#valorCop") != "") {

      this.equipoForm.patchValue({ valorUsd: '' });
      //$("#valorUsd").val("");
      //$("#valorUsd").text("");
    }
    else {
      if ($("#valorUsd").val() != "") {
        this.equipoForm.patchValue({ valorCop: '' });
        //$("#valorCop").val("");
        //$("#valorCop").text("");
      }
    }
  }

  formatValue(e: any) {
    let tag = (e.target || e.srcElement);
    let value = tag.value;
    value = Number(value.replace(/[^0-9.]/gi, ""));
    let maxValue = 9999999999;
    //console.log(maxValue);
    //9999999999
    if (value > maxValue) {
      this.modal.open("Error: El monto especificado supera el límite permitido ($10 mil millones)", "Alerta", Modal.Type.warning);
      tag.value = "";
      return false;
    }
    else {
      return FormatValue(e, "money");
    }
  }


  //Fin Eventos TextBox
}
export class Seleccionar {
  valor: string;
  name: string;
}
