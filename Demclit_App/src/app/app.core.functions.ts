import * as _ from 'underscore';
declare var $:any;
export function IsCharValid(e: any, type = undefined) {
  if(type == undefined){
    let tag = (e.target || e.srcElement);
    type  = tag.type || "text";
  }
  const types = { number: /[0-9]/, text: /[A-Za-zñÑ0-9,.\s-@]/, nit: /[0-9-]/, float: /[0-9.]/ }
  const keyNumerics = { 96: 0, 97: 1, 98: 2, 99: 3, 100: 4, 101: 5, 102: 6, 103: 7, 104: 8, 105: 9 }
  let IsValid = false;
  let pattern = types[type];
  let eKeyCode = e.charCode || e.keyCode;
  let eCharCode = String.fromCharCode(eKeyCode);

  //alert(patron + " | " + eventKeyCode + " | " + charCode + " | " + patron.test(charCode));
  switch (eKeyCode) {
      case 0:
      case 9:
      case 32:
          IsValid = true;
      break;
      default:
        IsValid = pattern.test(eCharCode);
      break;
  }
  return IsValid;
}

export function MaxLength(e: any) {
  let x = e.charCode || e.keyCode;
  let tag = (e.target || e.srcElement);
  let value = tag.value;
  let length = value.length;
  let inputMaxLength = (e.target.max.length > 0) ? e.target.max.length : e.target.maxLength || e.target.attributes['data-length'].nodeValue;
  return !(length >= inputMaxLength);
}

export function transform(val: Number): string {
  // Format the output to display any way you want here.
  // For instance:
  if (val !== undefined && val !== null) {
    return val.toLocaleString();
  } else {
    return '';
  }
}

export function FormatValue(e: any, type: string) {
  let tag = (e.target || e.srcElement);
  let value = tag.value;
  let eCharCode = e.key;
  switch (type) {
      case "money":
          let values = value.replace(/[^0-9.]/gi, "").split(".");
          let firstPart = values[0].split("").reverse();
          let secondPart = (values[1] || "");
          secondPart = (secondPart.length > 0 || eCharCode==".") ? "." + secondPart: secondPart;
          let numSeparators = (firstPart.length / 3) - 1;
          let tmpvalues = [];
          for(let index in firstPart){
            let currentValue = firstPart[index];
            let IsMil = (parseInt(index) + 1) % 3;
            tmpvalues.push(currentValue);
            if(IsMil == 0 && numSeparators > 0){
                tmpvalues.push(",");
                numSeparators--;
            }
          }
          tmpvalues = tmpvalues.reverse();
          tag.value = tmpvalues.join("") + secondPart;
      break;
      default:
        value = value;
      break;
  }

  return value;
}

enum ModalTypes{
  success = "<i class='material-icons left'>check_circle</i>",
  warning = "<i class='material-icons left'>error</i>"
}

export class Modal{
  static Type = ModalTypes;
  open(html: string, title:string = "Demclit", type:ModalTypes, showBtnDisagree: boolean = false, onClick: any = null){
    let modalTag = $('#appModal');
    modalTag.find('#appModal-title').text(title)
            .end().find('#appModal-body').html(type + html);

    if(showBtnDisagree){
      modalTag.find('#appModal-btnDisagree').removeClass("hide");
    }
    modalTag.modal('open');
  }
}


export class PagerService {
  getPager(totalItems: number, currentPage: number = 1, pageSize: number = 10) {
    // calculate total pages
    let totalPages = Math.ceil(totalItems / pageSize);

    let startPage: number, endPage: number;
    if (totalPages <= 10) {
      // less than 10 total pages so show all
      startPage = 1;
      endPage = totalPages;
    } else {
      // more than 10 total pages so calculate start and end pages
      if (currentPage <= 6) {
        startPage = 1;
        endPage = 10;
      } else if (currentPage + 4 >= totalPages) {
        startPage = totalPages - 9;
        endPage = totalPages;
      } else {
        startPage = currentPage - 5;
        endPage = currentPage + 4;
      }
    }

    // calculate start and end item indexes
    let startIndex = (currentPage - 1) * pageSize;
    let endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    // create an array of pages to ng-repeat in the pager control
    let pages = _.range(startPage, endPage + 1);

    // return object with all pager properties required by the view
    return {
      totalItems: totalItems,
      currentPage: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      startPage: startPage,
      endPage: endPage,
      startIndex: startIndex,
      endIndex: endIndex,
      pages: pages
    };
  }
}

