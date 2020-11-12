import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from 'src/_models/User';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}
  currentUser$: Observable<User>;


  constructor(private accountService: AccountService) {
    console.log(this.model);
  }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.setCurrentUser$;
  }
  login() {
    this.accountService.login(this.model).subscribe(res => {
      console.log(res);
    }, error => {
      console.log(error);
    });
  }
  logout() {
    this.accountService.logout();
  }
  getCurrentUser() {
    this.accountService.currentUserSource$.subscribe(user => {
    }, error => {
      console.log(error);
    });
  }
}
