import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule} from '@angular/router';
import { HttpModule } from '@angular/http';


import { AppComponent } from './components/app/app.component';
import { NavmenuComponent } from './components/navmenu/navmenu.component';
import { FooterComponent } from './components/footer/footer.component';
import { HomeComponent } from './components/home/home.component';
import { B2bVentaComponent } from './components/b2b-venta/b2b-venta.component';
import { PagoRegistroComponent } from './components/global/pago-registro/pago-registro.component';
import { DataService } from './services/data.service';
import { HttpService } from './services/http.service';
import { WebapiService } from './services/webapi.service';
import { ProyectosComponent } from './components/b2b/proyectos/proyectos.component';
import { ActivosComponent } from './components/b2b/activos/activos.component';
import { ActivosConsultaComponent } from './components/b2b/activos-consulta/activos-consulta.component';
import { ProyectosVerticalComponent } from './components/b2b/proyectos-vertical/proyectos-vertical.component';
import { ProyectosConsultaComponent } from './components/b2b/proyectos-consulta/proyectos-consulta.component';


@NgModule({
  declarations: [
    AppComponent,
    B2bVentaComponent,
    NavmenuComponent,
    FooterComponent,
    HomeComponent,
    PagoRegistroComponent,
    ProyectosComponent,
    ActivosComponent,
    ActivosConsultaComponent,
    ProyectosVerticalComponent,
    ProyectosConsultaComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
        { path: '', redirectTo: 'home', pathMatch: 'full' },
        { path: 'home', component: HomeComponent },
       // { path: 'b2b-venta', component: B2bVentaComponent },
        //{ path: 'pago-registro/:userLogin', component: PagoRegistroComponent },
        { path: 'b2b-proyectos/registro/:userLogin/:idProyecto', component: ProyectosComponent },
        // { path: 'b2b-activos/registro', component: ActivosComponent },
        // { path: 'b2b-activos/consulta', component: ActivosConsultaComponent },
         { path: 'b2b-proyectos/consulta/:userLogin', component: ProyectosConsultaComponent}
    ])
  ],
  providers: [
    DataService,
    HttpService,
    WebapiService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
