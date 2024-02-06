import { Component } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ApiService } from './services/api.service';
import { HttpHeaders } from '@angular/common/http';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'SignalRClient';

  allUsersData: any[] = [];
  user: USER = new USER();

  formValue !: FormGroup;

  private hubConnectionBuilder!: HubConnection;

  constructor(
    private api: ApiService,
    private fb: FormBuilder,
  ) { }

  ngOnInit(): void {
    this.formValue = this.fb.group({
      ID: [''],
      FNAME: [''],
      LNAME: [''],
      EMAIL: [''],
    })
    this.getAllUsers();
    this.hubConnectionBuilder = new HubConnectionBuilder()
      .withUrl('https://localhost:7060/offers')
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnectionBuilder
      .start()
      .then(() => console.log('Connection started.......!'))
      .catch(err => console.log(err));

    this.hubConnectionBuilder.on('SendOffersToUser', (result: any) => {
      this.allUsersData.push(result)
    });
    this.hubConnectionBuilder.on('UpdateOffersToUser', (result: any) => {
      console.log(result);
      this.updateItemToArray(result);
    })
    this.hubConnectionBuilder.on('RemoveOffersToUser', (result: any) => {
      // console.log(result);
      this.deleteItemFromArray(result);
    });
  }

  getAllUsers() {
    this.api.getAllUsers().subscribe(
      (res: any) => {
        this.allUsersData = res; // Assign the response directly to allUsersData
        // console.log(this.allUsersData);
      },
      (error: any) => {
        console.error(error);
      }
    );
  }

  addNewUser(form: any) {
    this.user.fname = form.value.FNAME;
    this.user.lname = form.value.LNAME;
    this.user.email = form.value.EMAIL;
    // console.log(form.value);

    if (form.value.ID == 0 || form.value.ID == null) {
      this.user.id = -1;
    } else {
      this.user.id = form.value.ID;
    }

    // console.log(this.user);

    this.api.addNewUser(this.user)
      .subscribe(
        (res: any) => {
          this.formValue.reset();

        },
        (error: any) => {
          console.error('Error adding user:', error);
        }
      );
  }

  editUser(user: any) {
    // console.log(user);
    this.formValue.setValue({
      ID: user.id,
      FNAME: user.fname,
      LNAME: user.lname,
      EMAIL: user.email,
    })
  }

  updateItemToArray(updatedUser: any): void {
    const index: number = this.allUsersData.findIndex((user: any) => user.id === updatedUser.id);
    if (index !== -1) {
      this.allUsersData[index] = updatedUser;
    }
  }

  deleteUser(user: any) {
    const cfr = confirm("are you sure you want to delete this user ?")
    if (cfr == true) {
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

      this.api.deleteUser(user, { headers })
        .subscribe((res: any) => {
        });
    }
    else {
      return
    }
  }

  deleteItemFromArray(userToRemove: any) {
    const index: number = this.allUsersData.findIndex(user => user.id === userToRemove.id);
    if (index !== -1) {
      this.allUsersData.splice(index, 1);
    }
  }

}


export class USER {
  id?: number;
  fname: string = "string";
  lname: string = "string";
  email: string = "string";
}