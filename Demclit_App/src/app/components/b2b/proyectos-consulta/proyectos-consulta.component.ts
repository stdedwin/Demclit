import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { WebapiService } from '../../../services/webapi.service';
import { PagerService, Modal } from '../../../app.core.functions';
import { Proyecto } from '../../../models/proyecto';
import { VerticalEquipo } from '../../../models/vertical-equipo';
import { ListaGenerica } from '../../../models/lista-generica';

import * as _ from 'underscore';

declare var $: any;
declare var M: any;

@Component({
  selector: 'app-proyectos-consulta',
  templateUrl: './proyectos-consulta.component.html',
  styleUrls: ['./proyectos-consulta.component.css']
})
export class ProyectosConsultaComponent implements OnInit {
  private pagerService: PagerService;
  private sub: any;
  private proyectos: Proyecto[];
  private listaGenerica: ListaGenerica[];
  private modal: Modal;
  consultaEliminar: String;
  userLogin: string;
  private verticalEquipo: VerticalEquipo;

  Consultar: boolean = false;
  Editar: boolean = false;
  Reservar: boolean = false;
  Ordenar: boolean = false;

  private allItemsProyectos: any[];
  pagerProyectos: any = {};
  pagedItemsProyectos: any[];

  constructor(private route: ActivatedRoute, private webapi: WebapiService) {
    this.pagerService = new PagerService();
    this.modal = new Modal();
    this.sub = this.route.params.subscribe(params => {
      this.userLogin = params['userLogin'] == "" ? "0" : params['userLogin'];
    });
    this.ConsultaMetodo("roles", this.userLogin);
  }

  ngOnInit() {
    
    this.ConsultarProyecto("ProyectoConsultaRoles", this.userLogin);
  }

  setPageProyectos(page: number) {
    if (page < 1 || page > this.pagerProyectos.totalPages) {
      return;
    }
    // get pager object from service
    this.pagerProyectos = this.pagerService.getPager(this.allItemsProyectos.length, page);
    // get current page of items
    this.pagedItemsProyectos = this.allItemsProyectos.slice(this.pagerProyectos.startIndex, this.pagerProyectos.endIndex + 1);
  }

  ConsultarProyecto(pConsulta: String, pFiltro: String): any {
    this.webapi.getData(pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: Proyecto[]) => {
        //console.log(resp);
        this.proyectos = resp;
        this.allItemsProyectos = this.proyectos;
        this.setPageProyectos(1);
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }

  nuevoProyecto() {
    window.location.href = "./b2b-proyectos/registro/"+ this.userLogin + "/" + "new";
  }

  editarProyecto(pIdProyecto: String) {

    window.location.href = "./b2b-proyectos/registro/"+ this.userLogin + "/" + pIdProyecto;
  }

  EliminarProyecto(pIdProyecto: string) {

    let msgConfirm = `¿Está seguro que desea eliminar el proyecto?`;
    if (confirm(msgConfirm)) {
      this.EliminarMetodo("VerticalesEquipoEliminar", pIdProyecto)

    }
  }
  EliminarMetodo(pConsulta: string, pFiltro: string = null): any {
    this.verticalEquipo = {} as VerticalEquipo;
    this.verticalEquipo.idProyecto = pFiltro;
    this.webapi.putData(pConsulta, this.verticalEquipo).subscribe(
      (resp: Boolean) => {
        this.ConsultarProyecto("ProyectoConsultaRoles", this.userLogin);
        this.modal.open("El registro se elimino con exito", "Resultado de Transacción", resp ? Modal.Type.success : Modal.Type.warning);
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }

  ConsultaMetodo(pConsulta: String, pFiltro: String = null) {
    this.webapi.getData("ListaGenerica/" + pConsulta + (pFiltro == null ? "" : ("/" + pFiltro))).subscribe(
      (resp: ListaGenerica[]) => {
        this.AsignarPermisos(resp)
      }
      , error => {
        this.modal.open("Error: No se pudo establecer conexión con el servicio. Por favor recargue la página", "Resultado de Transacción", Modal.Type.warning);
      }
    )
  }

  AsignarPermisos(data: ListaGenerica[]) {

    let listaConsulta = data.filter(u => u.Id == '45');
    if (listaConsulta.length > 0) {
      this.Consultar = true;
      this.Reservar = true;
      this.Editar = true;
      this.Ordenar = true;
    }

    // let listaConsulta = data.filter(u => u.Id == '45' || u.Id == '46' || u.Id == '47' || u.Id == '48' || u.Id == '49' || u.Id == '50');
    // if (listaConsulta.length > 0) {
    //   this.Consultar = true;
    // }
    // let listaReserva = data.filter(u => u.Id == '45' || u.Id == '46' || u.Id == '48' || u.Id == '50');
    // if (listaReserva.length > 0) {
    //   this.Reservar = true;
    // }
    // let listaEditar = data.filter(u => u.Id == '45' || u.Id == '46' );
    // if (listaEditar.length > 0) {
    //   this.Editar = true;
    // }
    // let listaOrdenar = data.filter(u => u.Id == '45' || u.Id == '49' || u.Id == '50' );
    // if (listaOrdenar.length > 0) {
    //   this.Ordenar = true;
    // }

  }


}

