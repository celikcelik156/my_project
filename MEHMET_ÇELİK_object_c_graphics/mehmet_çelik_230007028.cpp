#include <graphics.h>
#include <string.h>

int x,y,i=1,j=0,bul=0;

FILE *dosya,*yedekle;

char aranan1[30];

char aranan2[30];

struct kullanici {

	char ad[30];
	char soyad[30];
	char yas[30];
	char kullaniciadi[30];
	char sifre[30];

} giris[100];

void metin_kutusu_dis_cizgi (int xekseni, int yekseni, int genislik, int yukseklik) {

	genislik += xekseni;
	yukseklik += yekseni;

	setcolor(WHITE);
	setlinestyle(SOLID_LINE, 0, 1);
	rectangle(xekseni+2,yekseni+2,genislik-2,yukseklik-2);
	rectangle(xekseni,yekseni,genislik,yukseklik);
}

void metin (int xekseni, int yekseni, int yaziboyutu,const char *metin) {

	setcolor(WHITE);
	settextstyle(SANS_SERIF_FONT, 0, yaziboyutu);
	outtextxy(xekseni, yekseni,const_cast<char*>(metin));
}

void metin_kutusu1(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu,char aranan[30]) ;
void metin_kutusu2(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu,char aranan[30]) ;
void metin_kutusu(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu) ;
void giris_yap();
void kayit_ol();
void yedekleme();
void degistir();
void sifremi_unuttum();
void sifre_degis();
void hesap_sil();
void kisi_profili();
void profil();
void bildirimler();
void ana_sayfa();
void sayfa_2();
void menu_ici();

int main() {

	initwindow(639,479,"CelloTech");

	x=mousex();
	y=mousey();

	while(1) {
		profil();
		if(i==0) {
			while(1) {
				ana_sayfa();
				menu_ici();
			}
		}
	}
	getch();
	closegraph();
}

void metin_kutusu1(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu,char aranan[30]) {

	metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

	char metin1[20]="";
	int harfsirasi = 0;

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		if(kbhit()) {

			char tus = getch();

			if(harfsirasi>19) {
				break;
			}
			if(tus == 13) { //ASCII, 13 = Enter
				break;
			}
			if(tus == 8) { //ASCII, 8 = Backspace

				if(harfsirasi > 0) {
					harfsirasi--;
					metin1[harfsirasi] ='\0';
				}
			} else if(tus < 255) {

				metin1[harfsirasi] = tus;
				harfsirasi++;
				metin1[harfsirasi]='\0';
			}
		}

		metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

		int yaziyukseklik = textheight(metin1);
		int butonyukseklik = yukseklik - yekseni;
		float yaziy = yaziyukseklik + yekseni + ((butonyukseklik - yaziyukseklik) / 2);

		metin(xekseni+5,yekseni+5,yaziboyutu,metin1);

		if(x<50&&x>10&&y<50&&y>10) {

			cleardevice();
			profil();
		}

		if(x>getmaxx()/2-100&&x<getmaxx()/2-10&&y>getmaxy()/2+100&&y<getmaxy()/2+120) {

			sifremi_unuttum();
			giris_yap();
		}

	}
	strcpy(aranan,metin1);
}

void metin_kutusu2(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu,char aranan[30]) {

	metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

	char metin1[20]="";
	int harfsirasi = 0;

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);
		if(kbhit()) {
			char tus = getch();

			if(harfsirasi>19) {//
				break;
			}
			if(tus == 13) { //ASCII, 13 = Enter
				break;
			}
			if(tus == 8) { //ASCII, 8 = Backspace
				if(harfsirasi > 0) {
					harfsirasi--;
					metin1[harfsirasi] ='\0';
				}
			} else if(tus < 255) {
				metin1[harfsirasi] = tus;
				harfsirasi++;
				metin1[harfsirasi]='\0';
			}
		}
		metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

		int yaziyukseklik = textheight(metin1);
		int butonyukseklik = yukseklik - yekseni;
		float yaziy = yaziyukseklik + yekseni + ((butonyukseklik - yaziyukseklik) / 2);

		metin(xekseni+5,yekseni+5,yaziboyutu,metin1);

		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {
			cleardevice();

			profil();
		}
	}
	strcpy(aranan,metin1);
}

void metin_kutusu(int xekseni, int yekseni, int genislik, int yukseklik, int yaziboyutu) {

	metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

	char metin1[20]="";
	int harfsirasi = 0;

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);
		if(kbhit()) {
			char tus = getch();
			if(harfsirasi>19) {
				break;
			}
			if(tus == 13) { //ASCII, 13 = Enter
				break;
			}
			if(tus == 8) { //ASCII, 8 = Backspace

				if(harfsirasi > 0) {
					harfsirasi--;
					metin1[harfsirasi] = '\0';
				}

			} else {

				if(tus < 255) {

					metin1[harfsirasi] = tus;
					harfsirasi++;
					metin1[harfsirasi]='\0';
				}
			}

			metin_kutusu_dis_cizgi(xekseni,yekseni,genislik,yukseklik);

			int yaziyukseklik = textheight(metin1);
			int butonyukseklik = yukseklik - yekseni;
			float yaziY = yaziyukseklik + yekseni + ((butonyukseklik - yaziyukseklik) / 2);

			metin(xekseni+5,yekseni+5,yaziboyutu,metin1);
		}

		if(x<50&&x>10&&y<50&&y>10) {
			cleardevice();
			profil();
		}
	}
	fprintf(dosya, "%s ",metin1);
}

void giris_yap() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {//geri tuþu
			cleardevice();
			break;
		}

		settextstyle(BOLD_FONT,0,3);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-170,80,"~~GÝRÝÞ YAP~~");
		setcolor(WHITE);
		settextstyle(4,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-70,"*kullanýcý adý");
		rectangle(getmaxx()/2-100,getmaxy()/2-50,getmaxx()/2+103,getmaxy()/2-20);
		rectangle(getmaxx()/2-101,getmaxy()/2-51,getmaxx()/2+104,getmaxy()/2-19);
		outtextxy(getmaxx()/2-103,getmaxy()/2,"*þifre");
		rectangle(getmaxx()/2-100,getmaxy()/2+20,getmaxx()/2+103,getmaxy()/2+50);
		rectangle(getmaxx()/2-101,getmaxy()/2+19,getmaxx()/2+104,getmaxy()/2+51);
		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+100,getmaxy()/2+190,"*ENTER tuþuna basýnca yazma biter.");
		setcolor(WHITE);
		settextstyle(3,0,1);
		outtextxy(getmaxx()/2-100,getmaxy()/2+100,"*þifremi unuttum?");

		metin_kutusu1(getmaxx()/2-100,getmaxy()/2-50,203,30,1,aranan1);//kullanýcý adý

		metin_kutusu1(getmaxx()/2-100,getmaxy()/2+20,203,30,1,aranan2);//þifre

		if((dosya =fopen("kullanici_bilgileri.txt","r"))!=NULL) {//kullanýcý adý ve þifre doðruluk kontrolü

			while(!feof(dosya)) {

				fscanf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

				if(strcmp(aranan1,giris[j].kullaniciadi)==0 && strcmp(aranan2,giris[j].sifre)==0) {

					fclose(dosya);

					i=0;

					kisi_profili();

					break;
				}

				j=j+1;

			}

			setcolor(RED);
			settextstyle(3,0,2);
			outtextxy(getmaxx()/2-150,getmaxy()/2+100,"*kullanýcý adý veya þifre hatalý tekrar deneyin");
			setcolor(WHITE);

		}
		if(x>getmaxx()/2-100&&x<getmaxx()/2-10&&y>getmaxy()/2+100&&y<getmaxy()/2+120) {//þifremi unuttum

			sifremi_unuttum();

			giris_yap();
		}

		delay(1000);
	}
}

void kayit_ol() {

	dosya= fopen("kullanici_bilgileri.txt", "a");

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {//geri tuþu
			cleardevice();
			break;
		}

		settextstyle(BOLD_FONT,0,3);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-150,10,"~~KAYIT OL~~");
		setcolor(WHITE);

		settextstyle(4,0,1);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-103,getmaxy()/2-200,"*ad");
		outtextxy(getmaxx()/2-103,getmaxy()/2-120,"*soyad");
		outtextxy(getmaxx()/2-103,getmaxy()/2-40,"*yaþ");
		outtextxy(getmaxx()/2-103,getmaxy()/2+40,"*kullanýcý adý");
		outtextxy(getmaxx()/2-103,getmaxy()/2+120,"*þifre");

		setcolor(WHITE);
		rectangle(getmaxx()/2-101,getmaxy()/2-181,getmaxx()/2+104,getmaxy()/2-151);
		rectangle(getmaxx()/2-102,getmaxy()/2-182,getmaxx()/2+105,getmaxy()/2-152);

		rectangle(getmaxx()/2-101,getmaxy()/2-101,getmaxx()/2+104,getmaxy()/2-71);
		rectangle(getmaxx()/2-102,getmaxy()/2-102,getmaxx()/2+105,getmaxy()/2-72);

		rectangle(getmaxx()/2-101,getmaxy()/2-21,getmaxx()/2+104,getmaxy()/2+9);
		rectangle(getmaxx()/2-102,getmaxy()/2-22,getmaxx()/2+105,getmaxy()/2+8);

		rectangle(getmaxx()/2-101,getmaxy()/2+59,getmaxx()/2+104,getmaxy()/2+89);
		rectangle(getmaxx()/2-102,getmaxy()/2+58,getmaxx()/2+105,getmaxy()/2+88);

		rectangle(getmaxx()/2-101,getmaxy()/2+139,getmaxx()/2+104,getmaxy()/2+169);
		rectangle(getmaxx()/2-102,getmaxy()/2+138,getmaxx()/2+105,getmaxy()/2+168);

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+100,getmaxy()/2+190,"*ENTER tuþuna basýnca yazma biter.");

		if (dosya != NULL) {//kullanýcý bilgilerini kaydetme

			metin_kutusu(getmaxx()/2-100,getmaxy()/2-180,206,30,1);

			metin_kutusu(getmaxx()/2-100,getmaxy()/2-100,206,30,1);

			metin_kutusu(getmaxx()/2-100,getmaxy()/2-20,206,30,1);

			metin_kutusu(getmaxx()/2-100,getmaxy()/2+60,206,30,1);

			metin_kutusu(getmaxx()/2-100,getmaxy()/2+140,206,30,1);

			fprintf(dosya,"\n");

			fclose(dosya);

			outtextxy(getmaxx()/2+100,getmaxy()-20,"kayýt olundu");
			delay(1000);
			break;
		}
		delay(1000);
	}

}

void yedekleme() {

	if((yedekle=fopen("yedek.txt","w"))!=NULL) {//kullanýcý bilgilerini yedekle dosyasýna aktarma

		if((dosya=fopen("kullanici_bilgileri.txt","r"))!=NULL) {

			while(!feof(dosya)) {

				fscanf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

				fprintf(yedekle,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

			}
		}
	}
	remove("kullanici_bilgileri.txt");//kullanýcý bilgileri dosyasýný silme

	fclose(dosya);
	fclose(yedekle);
}

void degistir() {//silme komutu

	if((dosya=fopen("kullanici_bilgileri.txt","w"))!=NULL) {

		if((yedekle=fopen("yedek.txt","r"))!=NULL) {

			while(!feof(yedekle)) {

				fscanf(yedekle,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

				if(strcmp(aranan1,giris[j].kullaniciadi)==0 && strcmp(aranan2,giris[j].sifre)==0) {//kullanýcý adý ve þifre doðruluk hesap silinir

					outtextxy(getmaxx()/2-150,getmaxy()/2+100,"*hesap silindi");
					delay(2000);
					bul=1;
					setcolor(WHITE);
					continue;//atlar

				} else {

					fprintf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);
				}

				j=j+1;
			}
		}
	}
	if(bul==0) {//kullanýcý adý veya þifre hatalýysa yeniden isteyecektir.

		setcolor(RED);
		settextstyle(3,0,2);
		outtextxy(getmaxx()/2-150,getmaxy()/2+100,"*kullanýcý adý veya þifre hatalý tekrar deneyin");
		delay(3000);
		setcolor(WHITE);
		hesap_sil();

	}

	if(bul==1) {//kullanýcý adý veya þifre hatalý deðilse sisteme girecek

		remove("yedek.txt");
		fclose(dosya);
		fclose(yedekle);
		profil();

	}

	remove("yedek.txt");//yedek dosyasýný silme.

	fclose(dosya);
	fclose(yedekle);
}

void sifremi_unuttum() {

	dosya=fopen("kullanici_bilgileri.txt","r");

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {//geri tuþu.
			cleardevice();
			break;
		}

		settextstyle(BOLD_FONT,0,3);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-170,80,"~~ÞÝFREMÝ UNUTTUM~~");

		setcolor(WHITE);
		settextstyle(3,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-100,"*hesabýnýzýn kullanýcý adýný giriniz");

		setcolor(YELLOW);
		settextstyle(4,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-70,"*kullanýcý adý");

		setcolor(WHITE);
		rectangle(getmaxx()/2-100,getmaxy()/2-50,getmaxx()/2+103,getmaxy()/2-20);
		rectangle(getmaxx()/2-101,getmaxy()/2-51,getmaxx()/2+104,getmaxy()/2-19);

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+80,getmaxy()/2+150,"*ENTER tuþuna basýnca yazma biter.");

		metin_kutusu2(getmaxx()/2-100,getmaxy()/2-50,203,30,1,aranan1);

		if((dosya =fopen("kullanici_bilgileri.txt","r"))!=NULL) {//þifre deðiþtirme komutu

			while(!feof(dosya)) {

				fscanf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

				if(strcmp(aranan1,giris[j].kullaniciadi)==0) {

					yedekleme();
					sifre_degis();
					break;
				}
			}

			setcolor(RED);
			settextstyle(3,0,2);
			outtextxy(getmaxx()/2-150,getmaxy()/2+100,"*kullanýcý adý hatalý tekrar deneyin");
			delay(1000);
			setcolor(WHITE);
		}
		delay(100);
	}
}

void sifre_degis() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {//geri tuþu
			cleardevice();
			break;
		}

		settextstyle(BOLD_FONT,0,3);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-170,80,"~~ÞÝFREMÝ UNUTTUM~~");

		setcolor(WHITE);
		settextstyle(3,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-100,"*yeni þifrenizi girin");

		setcolor(YELLOW);
		settextstyle(4,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-70,"*þifre");

		setcolor(WHITE);
		rectangle(getmaxx()/2-100,getmaxy()/2-50,getmaxx()/2+103,getmaxy()/2-20);
		rectangle(getmaxx()/2-101,getmaxy()/2-51,getmaxx()/2+104,getmaxy()/2-19);

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+80,getmaxy()/2+150,"*ENTER tuþuna basýnca yazma biter.");


		if((dosya=fopen("kullanici_bilgileri.txt","w"))!=NULL) { //þifre deðiþtirme

			if((yedekle=fopen("yedek.txt","r"))!=NULL) {

				while(!feof(yedekle)) {

					fscanf(yedekle,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

					if(strcmp(aranan1,giris[j].kullaniciadi)==0) {//kullanýcý adý kontrolü

						metin_kutusu2(getmaxx()/2-100,getmaxy()/2-50,203,30,1,giris[j].sifre);//doðruysa tekrar isteyecek

						bul=1;
					}

					fprintf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

					j=j+1;
				}
			}
		}

		if(bul==1) {

			remove("yedek.txt");
			fclose(dosya);
			fclose(yedekle);
			profil();
		}

		remove("yedek.txt");
		fclose(dosya);
		fclose(yedekle);
		delay(100);
	}
}

void hesap_sil() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		setcolor(YELLOW);
		circle(30,30,20);
		line(20,30,40,30);
		line(20,30,30,20);
		line(20,30,30,40);
		setcolor(WHITE);

		if(x<50&&x>10&&y<50&&y>10) {//geri alma
			cleardevice();
			break;
		}

		settextstyle(BOLD_FONT,0,3);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-170,80,"~~HESAP SÝL~~");

		settextstyle(2,0,5);
		outtextxy(getmaxx()/2-103,getmaxy()/2-90,"*silmek istediðiniz hesabýn kullanýcý adý ve þifresini giriniz");

		settextstyle(4,0,1);
		outtextxy(getmaxx()/2-103,getmaxy()/2-70,"*kullanýcý adý");
		outtextxy(getmaxx()/2-103,getmaxy()/2,"*þifre");

		setcolor(WHITE);
		rectangle(getmaxx()/2-100,getmaxy()/2-50,getmaxx()/2+103,getmaxy()/2-20);
		rectangle(getmaxx()/2-101,getmaxy()/2-51,getmaxx()/2+104,getmaxy()/2-19);

		rectangle(getmaxx()/2-100,getmaxy()/2+20,getmaxx()/2+103,getmaxy()/2+50);
		rectangle(getmaxx()/2-101,getmaxy()/2+19,getmaxx()/2+104,getmaxy()/2+51);

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+100,getmaxy()/2+190,"*ENTER tuþuna basýnca yazma biter.");

		metin_kutusu2(getmaxx()/2-100,getmaxy()/2-50,203,30,1,aranan1);

		metin_kutusu2(getmaxx()/2-100,getmaxy()/2+20,203,30,1,aranan2);


		if((dosya =fopen("kullanici_bilgileri.txt","r"))!=NULL) {

			while(!feof(dosya)) {

				fscanf(dosya,"%s %s %s %s %s \n",giris[j].ad,giris[j].soyad,giris[j].yas,giris[j].kullaniciadi,giris[j].sifre);

				if(strcmp(aranan1,giris[j].kullaniciadi)==0 && strcmp(aranan2,giris[j].sifre)==0) {//doðruysa deðiþtir fonk. gönderir

					fclose (dosya);
					yedekleme();
					degistir();
					break;
				}
			}

			setcolor(RED);//doðru deðilse hesap_sil fonk. tekrarlayacaktýr
			settextstyle(3,0,2);
			outtextxy(getmaxx()/2-150,getmaxy()/2+100,"*kullanýcý adý veya þifre hatalý tekrar deneyin");
			delay(1000);
			setcolor(WHITE);
		}
		delay(100);
	}
}

void kisi_profili() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();

		setfillstyle(WIDE_DOT_FILL,WHITE);
		bar(getmaxx()/2-140,0,getmaxx()/2+190,479);


		setcolor(WHITE);
		rectangle(10,7,130,45);
		rectangle(11,8,131,46);
		rectangle(15,10,45,40 );
		rectangle(16,11,46,41);
		line(20,15,40,15);
		line(20,16,40,16);
		line(20,25,40,25);
		line(20,26,40,26);
		line(20,35,40,35);
		line(20,36,40,36);
		settextstyle(BOLD_FONT,0,1);
		outtextxy(55,20,"MENÜ");

		settextstyle(BOLD_FONT,0,4);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-110,50,"~~PROFÝLÝM~~");

		settextstyle(2,0,5);
		setcolor(WHITE);
		outtextxy(getmaxx()/2-100,170,"kullanýcý adý:");
		settextstyle(6,0,1);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-110,180,const_cast<char*>(giris[j].kullaniciadi));

		settextstyle(2,0,5);
		setcolor(WHITE);
		outtextxy(getmaxx()/2-10,220,"ad:");
		settextstyle(6,0,1);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+15,215,const_cast<char*>(giris[j].ad));

		settextstyle(2,0,5);
		setcolor(WHITE);
		outtextxy(getmaxx()/2-10,250,"soyad:");
		settextstyle(6,0,1);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+25,245,const_cast<char*>(giris[j].soyad));

		settextstyle(2,0,5);
		setcolor(WHITE);
		outtextxy(getmaxx()/2-10,280,"yas:");
		settextstyle(6,0,1);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2+25,275,const_cast<char*>(giris[j].yas));

		settextstyle(0,0,1);
		outtextxy(getmaxx()/2-95,getmaxy()-90,"CIKIS YAP");
		rectangle(getmaxx()/2-100,getmaxy()-100,getmaxx()/2,getmaxy()-70);

		setcolor(WHITE);
		settextstyle(DEFAULT_FONT,0,1);
		line(getmaxx()/2-140,0,getmaxx()/2-140,getmaxy());
		line(getmaxx()/2+190,0,getmaxx()/2+190,getmaxy());
		setfillstyle(SOLID_FILL,WHITE);
		fillellipse(getmaxx()/2-80,getmaxy()/2+10,35,35);
		setfillstyle(SOLID_FILL,LIGHTGRAY);
		fillellipse(getmaxx()/2-80,getmaxy()/2,10,10);
		fillellipse(getmaxx()/2-80,getmaxy()/2+25,15,15);
		bar(getmaxx()/2-95,getmaxy()/2+25,getmaxx()/2-65,getmaxy()/2+40);

		if(x<getmaxx()/2&&x>getmaxx()/2-100&&y<getmaxy()-60&&y>getmaxy()-100) {//çýkýþ yap tuþu

			i=1;
			profil();
		}

		if(x<131&&y<46&&x>10&&y>7) {//menü tuþu

			setcolor(WHITE);
			menu_ici();
		}

		delay(1000);
	}
}

void profil() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();
		settextstyle(BOLD_FONT,0,4);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-130,50,"~~BAÞLANGIÇ~~");

		settextstyle(DEFAULT_FONT,0,1);
		setcolor(WHITE);
		line(getmaxx()/2-140,0,getmaxx()/2-140,getmaxy());
		line(getmaxx()/2+190,0,getmaxx()/2+190,getmaxy());
		setfillstyle(SOLID_FILL,WHITE);
		fillellipse(getmaxx()/2+20,getmaxy()/2-70,70,70);
		setfillstyle(SOLID_FILL,LIGHTGRAY);
		fillellipse(getmaxx()/2+20,getmaxy()/2-100,20,20);
		fillellipse(getmaxx()/2+20,getmaxy()/2-52,30,30);
		bar(getmaxx()/2-10,getmaxy()/2-52,getmaxx()/2+50,getmaxy()/2-10);

		outtextxy(getmaxx()/2-90,getmaxy()/2+33,"GIRIS YAP");
		rectangle(getmaxx()/2-100,getmaxy()/2+20,getmaxx()/2,getmaxy()/2+50);
		rectangle(getmaxx()/2-101,getmaxy()/2+21,getmaxx()/2+1,getmaxy()/2+51);

		outtextxy(getmaxx()/2+67,getmaxy()/2+33,"KAYIT OL");
		rectangle(getmaxx()/2+50,getmaxy()/2+20,getmaxx()/2+150,getmaxy()/2+50);
		rectangle(getmaxx()/2+49,getmaxy()/2+21,getmaxx()/2+151,getmaxy()/2+51);

		outtextxy(getmaxx()/2-15,getmaxy()/2+93,"HESAP SIL");
		rectangle(getmaxx()/2-25,getmaxy()/2+80,getmaxx()/2+75,getmaxy()/2+110);
		rectangle(getmaxx()/2-24,getmaxy()/2+81,getmaxx()/2+76,getmaxy()/2+111);

		//hesap sil
		if(x<getmaxx()/2+76&&x>getmaxx()/2-25&&y<getmaxy()/2+111&&y>getmaxy()/2+80) {
			hesap_sil();
		}
		//giriþ yap
		if(i>0)
			if(x<getmaxx()/2&&y<getmaxy()/2+50&&x>getmaxx()/2-100&&y>getmaxy()/2+20) {
				giris_yap ();
			}
		//kayýt ol
		if(x<getmaxx()/2+150&&y<getmaxy()/2+50&&x>getmaxx()/2+50&&y>getmaxy()/2+20) {
			kayit_ol();
		}
		delay(1000);
	}
}

void bildirimler() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();

		setfillstyle(WIDE_DOT_FILL,WHITE);
		bar(getmaxx()/2-140,0,639,479);

		setcolor(WHITE);
		rectangle(10,7,130,45);
		rectangle(11,8,131,46);
		rectangle(15,10,45,40 );
		rectangle(16,11,46,41);
		line(20,15,40,15);
		line(20,16,40,16);
		line(20,25,40,25);
		line(20,26,40,26);
		line(20,35,40,35);
		line(20,36,40,36);
		settextstyle(BOLD_FONT,0,1);
		outtextxy(55,20,"MENÜ");

		line(getmaxx()/2-140,0,getmaxx()/2-140,getmaxy());
		settextstyle(BOLD_FONT,HORIZ_DIR,4);
		setcolor(YELLOW);
		outtextxy(getmaxx()/2-50,30,"BÝLDÝRÝMLER");
		setcolor(WHITE);

		setfillstyle(SOLID_FILL,YELLOW);
		fillellipse(getmaxx()/2+70,getmaxy()/2,20,20);
		setfillstyle(SOLID_FILL,LIGHTGRAY);
		fillellipse(getmaxx()/2+70,getmaxy()/2-50,40,40);
		fillellipse(getmaxx()/2+70,getmaxy()/2-90,6,6);

		for(int z=0; z<50; z++) {
			setcolor(LIGHTGRAY);
			line(getmaxx()/2+30-z,getmaxy()/2-50+z,getmaxx()/2+110+z,getmaxy()/2-50+z);
		}

		settextstyle(4,0,1);
		outtextxy(getmaxx()/2-25,getmaxy()/2+30,"BÝLDÝRÝM YOK");
		outtextxy(getmaxx()/2-5,getmaxy()/2+60,const_cast<char*>(strupr(giris[j].ad)));

		if(x<131&&y<46&&x>10&&y>7) {//menü tuþu
			setcolor(WHITE);
			menu_ici();
		}
		delay(1000);
	}
}

void ana_sayfa() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();

		setfillstyle(WIDE_DOT_FILL,GREEN);
		bar(0,0,609,479);

		setcolor(WHITE);
		rectangle(10,7,130,45);
		rectangle(11,8,131,46);
		rectangle(15,10,45,40 );
		rectangle(16,11,46,41);
		line(20,15,40,15);
		line(20,16,40,16);
		line(20,25,40,25);
		line(20,26,40,26);
		line(20,35,40,35);
		line(20,36,40,36);
		settextstyle(BOLD_FONT,0,1);
		outtextxy(55,20,"MENÜ");

		settextstyle(BOLD_FONT,HORIZ_DIR,4);
		setcolor(YELLOW);
		outtextxy(210,25,"CelloTech");

		setcolor(WHITE);
		settextstyle(6,0,1);
		outtextxy(20,60,"   **CelloTech'in misyonu, dünya genelindeki herkesin ");
		outtextxy(20,90,"kodlama becerilerini geliþtirmesine olanak saðlayan");
		outtextxy(20,120,"eðitim ve araçlar sunmaktadýr.Biz insanlarýn teknolojiye");
		outtextxy(20,150,"uyum saðlamasýný kolaylaþtýrmak ve yaratýcý düþünmeyi ");
		outtextxy(20,180,"teþvik etmek için en iyi eðitim kaynaklarýný sunmaktayýz.");
		outtextxy(20,220,"   **CelloTech'in vizyonu, her bireyin potansiyelini ");
		outtextxy(20,250,"keþfedip gerçekleþtirmesine yardýmcý olan, eriþilebilir");
		outtextxy(20,280,"ve etkili bir þekilde öðrenmeyi teþvik eden bir dünya");
		outtextxy(20,310,"yaratmaktýr. Gelecekte, her yaþ, cinsiyet ve coðrafyadan");
		outtextxy(20,340,"insanlarýn teknolojiyi kucaklamalarýna ve sýkýntýsýz ");
		outtextxy(20,370,"bir þekilde kullanmalarýna yardýmcý olacak çözümler  ");
		outtextxy(20,400,"sunmayý hedefliyoruz.");

		line(getmaxx()-30,0 ,getmaxx()-30,getmaxy());//kaydýrma yeri
		line(getmaxx()-30,20,getmaxx(),20);
		line(getmaxx()-30,getmaxy()-20,getmaxx(),getmaxy()-20);
		line(getmaxx()-25,15,getmaxx()-5,15);
		line(getmaxx()-25,15,getmaxx()-15,5);
		line(getmaxx()-15,5,getmaxx()-5,15);
		line(getmaxx()-25,getmaxy()-15,getmaxx()-5,getmaxy()-15);
		line(getmaxx()-25,getmaxy()-15,getmaxx()-15,getmaxy()-5);
		line(getmaxx()-15,getmaxy()-5,getmaxx()-5,getmaxy()-15);

		rectangle(getmaxx()-25,25,getmaxx()-5,getmaxy()/2-5);//kaydýrma tuþu
		line (getmaxx()-22,getmaxy()/4-5,getmaxx()-8,getmaxy()/4-5);
		line (getmaxx()-22,getmaxy()/4,getmaxx()-8,getmaxy()/4);
		line (getmaxx()-22,getmaxy()/4+5,getmaxx()-8,getmaxy()/4+5);

		settextstyle(0,0,1);
		outtextxy(5,getmaxy()-20,"sayfa 1");

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()-250,getmaxy()-20,"*designed and produced by MEHMET ÇELÝK");
		setcolor(WHITE);

		if(x<getmaxx()&&x>getmaxx()-30&&y<getmaxy()&&y>getmaxy()/2) {//kaydýrma

			sayfa_2();
		}
		if(x<131&&y<46&&x>10&&y>7) {//menü tuþu

			setcolor(WHITE);
			menu_ici();
		}
		delay(1000);
	}
}

void sayfa_2() {

	while(1) {

		getmouseclick(WM_LBUTTONDOWN,x,y);

		cleardevice();

		setfillstyle(WIDE_DOT_FILL,GREEN);
		bar(0,0,609,479);

		setcolor(WHITE);
		rectangle(10,7,130,45);
		rectangle(11,8,131,46);
		rectangle(15,10,45,40 );
		rectangle(16,11,46,41);
		line(20,15,40,15);
		line(20,16,40,16);
		line(20,25,40,25);
		line(20,26,40,26);
		line(20,35,40,35);
		line(20,36,40,36);
		settextstyle(BOLD_FONT,0,1);
		outtextxy(55,20,"MENÜ");

		settextstyle(6,0,1);
		outtextxy(20,60,"**CelloTech'in Deðerleri:*");
		outtextxy(10,90,"-Sürekli öðrenme ve geliþimde açýk olmak.");
		outtextxy(20,120,"-Topluluk ile etkileþimde bulunmak ve destek olmak.");
		outtextxy(20,150,"-Kaliteli eðitim ve çözümler sunmaya odaklanmak.");
		outtextxy(20,180,"- Dürüst ve þeffaf bir þekilde hareket etmek.");
		outtextxy(20,220,"**CelloTech'in *Tarihçesi:*");
		outtextxy(20,250,"2024 yýlýnda genç bir giriþimci tarafýndan küçük bir garaj");
		outtextxy(20,280,"ofisinde kuruldu.Ýlk baþta sadece birkaç online kodlama ");
		outtextxy(20,310,"kursu ile iþe baþlayan þirket hýzla büyüyerek daha geniþ ");
		outtextxy(20,340,"eðitim kaynaklarý ve interaktif uygulamalar geliþtirmeye");
		outtextxy(20,370,"baþladý. Þimdi ise öðrencilerin kariyerlerinde baþarýlý ");
		outtextxy(20,400,"olmalarý için destek saðlamaya devam etmektedir. ");

		line(getmaxx()-30,0,getmaxx()-30,getmaxy());//kaydýrma yeri
		line(getmaxx()-30,20,getmaxx(),20);
		line(getmaxx()-30,getmaxy()-20,getmaxx(),getmaxy()-20);
		line(getmaxx()-25,15,getmaxx()-5,15);
		line(getmaxx()-25,15,getmaxx()-15,5);
		line(getmaxx()-15,5,getmaxx()-5,15);
		line(getmaxx()-25,getmaxy()-15,getmaxx()-5,getmaxy()-15);
		line(getmaxx()-25,getmaxy()-15,getmaxx()-15,getmaxy()-5);
		line(getmaxx()-15,getmaxy()-5,getmaxx()-5,getmaxy()-15);

		rectangle(getmaxx()-25,getmaxy()/2+5,getmaxx()-5,getmaxy()-25);//kaydýrma tuþu
		line (getmaxx()-22,getmaxy()/2+getmaxy()/4-5,getmaxx()-8,getmaxy()/2+getmaxy()/4-5);
		line (getmaxx()-22,getmaxy()/2+getmaxy()/4,getmaxx()-8,getmaxy()/2+getmaxy()/4);
		line (getmaxx()-22,getmaxy()/2+getmaxy()/4+5,getmaxx()-8,getmaxy()/2+getmaxy()/4+5);

		settextstyle(0,0,1);
		outtextxy(5,getmaxy()-20,"sayfa 2");

		settextstyle(2,0,5);
		setcolor(YELLOW);
		outtextxy(getmaxx()-250,getmaxy()-20,"*designed and produced by MEHMET ÇELÝK");
		setcolor(WHITE);

		if(x<getmaxx()&&x>getmaxx()-30&&y<getmaxy()/2&&y>0) {//kaydýrma

			ana_sayfa();
		}
		if(x<131&&y<46&&x>10&&y>7) {

			menu_ici();
		}
		delay(1000);
	}
}

void menu_ici() {
	while(1) {
		getmouseclick(WM_LBUTTONDOWN,x,y);

		setfillstyle(WIDE_DOT_FILL,RED);
		bar(10,7,171,191);

		setcolor(WHITE);
		rectangle(10,7,130,45);
		rectangle(11,8,131,46);
		rectangle(15,10,45,40 );
		rectangle(16,11,46,41);
		line(20,15,40,15);
		line(20,16,40,16);
		line(20,25,40,25);
		line(20,26,40,26);
		line(20,35,40,35);
		line(20,36,40,36);
		settextstyle(BOLD_FONT,0,1);
		outtextxy(55,20,"MENÜ");

		rectangle(5,2,170,190);
		rectangle(4,1,171,191);

		rectangle(10,60,164,90);
		rectangle(11,61,165,91);
		outtextxy(20,68,"ANA SAYFA");

		rectangle(10,100,164,130);
		rectangle(11,101,165,131);
		outtextxy(20,108,"BÝLDÝRÝMLER");

		rectangle(10,140,164,170);
		rectangle(11,141,165,171);
		outtextxy(20,148,"PROFÝLÝM");

		if(x<131&&y<46&&x>10&&y>7) {//menü tusu
			break;
		}

		//ana sayfa
		if(x<165&&y<90&&x>10&&y>61) {
			cleardevice();
			rectangle(10,7,130,45);
			rectangle(11,8,131,46);
			rectangle(15,10,45,40);
			rectangle(16,11,46,41);
			line(20,15,40,15);
			line(20,16,40,16);
			line(20,25,40,25);
			line(20,26,40,26);
			line(20,35,40,35);
			line(20,36,40,36);
			settextstyle(BOLD_FONT,0,1);
			outtextxy(55,20,"MENÜ");
			delay(100);

			ana_sayfa();
		}

		//bildirimler
		if(x<165&&y<131&&x>10&&y>100) {
			cleardevice();
			rectangle(10,7,130,45);
			rectangle(11,8,131,46);
			rectangle(15,10,45,40);
			rectangle(16,11,46,41);
			line(20,15,40,15);
			line(20,16,40,16);
			line(20,25,40,25);
			line(20,26,40,26);
			line(20,35,40,35);
			line(20,36,40,36);
			settextstyle(BOLD_FONT,0,1);
			outtextxy(55,20,"MENÜ");
			delay(100);

			bildirimler();
		}

		//profil
		if(x<165&&y<171&&x>10&&y>140) {
			cleardevice();
			rectangle(10,7,130,45);
			rectangle(11,8,131,46);
			rectangle(15,10,45,40);
			rectangle(16,11,46,41);
			line(20,15,40,15);
			line(20,16,40,16);
			line(20,25,40,25);
			line(20,26,40,26);
			line(20,35,40,35);
			line(20,36,40,36);
			settextstyle(BOLD_FONT,0,1);
			outtextxy(55,20,"MENÜ");
			delay(100);

			if(i==1)
				profil();
			else
				kisi_profili();
		}
		delay(1000);
	}
}
