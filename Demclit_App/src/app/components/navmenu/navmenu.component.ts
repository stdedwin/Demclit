import { Component, OnInit } from '@angular/core';

declare var jQuery: any;
declare var $: any;
declare var M: any;

@Component({
  selector: 'nav-menu',
  templateUrl: './navmenu.component.html',
  styleUrls: ['./navmenu.component.css']
})

export class NavmenuComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit() {
      $(document).ready(function () {
          $(".dropdown-trigger").dropdown();
          $('.sidenav').sidenav();

          
      });
  }
}
