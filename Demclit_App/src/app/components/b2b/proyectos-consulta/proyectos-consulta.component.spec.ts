import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProyectosConsultaComponent } from './proyectos-consulta.component';

describe('ProyectosConsultaComponent', () => {
  let component: ProyectosConsultaComponent;
  let fixture: ComponentFixture<ProyectosConsultaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProyectosConsultaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProyectosConsultaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
