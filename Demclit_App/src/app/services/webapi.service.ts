import { Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Pago } from '../models/pago';
import { User } from '../models/user';
const testMode = true;
// PAgos
// const URLAPI = testMode ? "http://localhost:2250/API.svc/" : "http://10.81.163.252/WCF_Demclit/API.svc/";
// Proyectos
const URLAPI = testMode ? "http://localhost:2250/API.svc/" : "http://10.81.163.252/DemclitB2B_WCF/API.svc/";
//console.log(URLAPI);
@Injectable()
export class WebapiService {
  private  urlBase: string;
  private items: any[] = [];
  response: any;

  constructor(private http: HttpService) { }

  getData(uri: string){
    this.urlBase = URLAPI + uri;
    return this.http.getData(this.urlBase);
  }

  sendData(uri: string, parametros:any = null){
    
    this.urlBase = URLAPI + uri;
    console.log('peticion '+this.urlBase);
    return this.http.sendData(this.urlBase, parametros);
  }

  putData(uri: string, parametros:any = null){
    this.urlBase = URLAPI + uri;
    return this.http.sendData(this.urlBase, parametros);
  }

  getFile(uri: string){
    this.urlBase = URLAPI + uri;
    return window.open(this.urlBase,"_self");
  }
}
