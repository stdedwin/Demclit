import { Injectable } from '@angular/core';
import {Pago} from '../models/pago'
import { User } from '../models/user';

@Injectable()
export class DataService {
  private user: User[] = [{codAsesor: "123", userLogin: "jcortes", codPunto: "654"}
                          , {codAsesor: "654", userLogin: "motta", codPunto: "789"}];

  constructor() { }

  getData(){
    return this.user;
  }
}
