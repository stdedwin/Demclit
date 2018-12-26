import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProyectosVerticalComponent } from './proyectos-vertical.component';

describe('ProyectosVerticalComponent', () => {
  let component: ProyectosVerticalComponent;
  let fixture: ComponentFixture<ProyectosVerticalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProyectosVerticalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProyectosVerticalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
