<?php
header("Access-Control-Allow-Origin: *");
$credentials = [
            'username' => 'dan',
            'password' => '123',
            'user_id' => "user-from-php"
        ];
echo json_encode($credentials);
