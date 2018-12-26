import { Component, OnInit } from '@angular/core';

declare var $: any;
declare var M: any;

@Component({
  selector: 'app-b2b-venta',
  templateUrl: './b2b-venta.component.html',
  styleUrls: ['./b2b-venta.component.css']
})
export class B2bVentaComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    $(document).ready(function () {
        M.updateTextFields();
        $('.collapsible').collapsible({ accordion: false});
    });
  }
}
