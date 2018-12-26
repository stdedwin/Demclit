import { Component, OnInit } from '@angular/core';
declare var $:any;
declare var M:any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit{
  navMenuVisible:boolean = false;
  footerVisible:boolean = false;

  ngOnInit(){
    $('#appModal').modal();
  }
}
