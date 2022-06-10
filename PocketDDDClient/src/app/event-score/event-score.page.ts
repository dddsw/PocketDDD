import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { CurrentUserContext } from '../models/clientData';
import { LocalDataService } from '../services/localData';
import { SyncService } from '../services/syncService';

@Component({
    selector: 'app-event-score',
    templateUrl: './event-score.page.html',
    styleUrls: ['./event-score.page.scss'],
})
export class EventScorePage {
    currentUser: CurrentUserContext;
    syncState = "Pending...";
    pendingDataCount = 0;
    currentScore = 0;

    constructor(private localData: LocalDataService, private syncService: SyncService, private modalController: ModalController) { 

    }

    ionViewWillEnter(){
        this.currentUser = this.localData.getCurrentUser();
        this.handleSync();
    }


    updateGameScore = () => this.currentScore = this.localData.getEventScore();

    updatePendingCount = () => {
        var ses = this.localData.getAllPendingSessionFeedbackData().length;
        var evt = this.localData.getAllPendingEventFeedbackData().length;

        this.pendingDataCount = ses + evt;
    }

    handleCancel() {
        this.modalController.dismiss({
            'dismissed': true
        });
    }

    async handleSync(){
        this.syncState = "Syncing...";

        try {
            await this.syncService.SyncAll();
            this.syncState = "Success"
        } catch (error) {
            this.syncState = "Failed"
        } finally {
            this.updatePendingCount();
        }
    }
}