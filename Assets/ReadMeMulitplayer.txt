Pour le multiplayer :

INPUT:
- Vous pouvez gerer vos inputs dans l'onglet Input Manager de Edit/Project Settings 
- Une fois vos inputs bien nommés (actionp1, horizontalP1 et horizontalP2 par exemple) vous pouvez les utiliser avec les fonction Input.GetButtonDown("actionp1"), Input.GetButtonUp("actionp1"), ou Input.GetAxis("Horizontalp1") pour avoir une valeur numérique entre 0 et 1
- Vous pouvez enregistrer le string que vous entrez dans votre GetButton dans une variable pour pouvoir la changer dans l'inspecteur en fonction du joueur. 

CAMERA:
- Vous pouvez gerer l'affichage de votre camera (ce qu'elle rend sur l'ecran) dans l'ongle Output de la camera
- Vous pouvez dupliquer votre Camera et vos cinemachine Camera mais faites attention a 
	- bien faire que vos camera n'aient pas le meme output
	- Faire attention a vos channel de cineamchine (cinemachineBrain 1 a le meme channel que cinemachineCamera 1 et pareil pour la 2)
	- N'oubliez pas de changer la target de follow camera de votre cinemachine pour le joueur 2
- N'oubliez pas de dupliquer l'UI 

Voila voila 
