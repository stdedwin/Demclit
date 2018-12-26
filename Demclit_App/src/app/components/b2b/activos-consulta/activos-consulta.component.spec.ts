import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivosConsultaComponent } from './activos-consulta.component';

describe('ActivosConsultaComponent', () => {
  let component: ActivosConsultaComponent;
  let fixture: ComponentFixture<ActivosConsultaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActivosConsultaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActivosConsultaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
