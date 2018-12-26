import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { B2bVentaComponent } from './b2b-venta.component';

describe('B2bVentaComponent', () => {
  let component: B2bVentaComponent;
  let fixture: ComponentFixture<B2bVentaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ B2bVentaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(B2bVentaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
